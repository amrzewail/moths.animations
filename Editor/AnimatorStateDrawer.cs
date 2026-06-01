using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Moths.Animations
{
    using static Moths.Animations.AnimatorPlayer;
    using static Moths.Animations.AnimatorStateReferences;
    using AnimatorController = UnityEditor.Animations.AnimatorController;
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(Moths.Animations.AnimatorState))]
    public class AnimatorStateDrawer : PropertyDrawer
    {
        private static Dictionary<AnimatorController, AnimatorStateReferences> _animators = new();

        public static bool ShouldFetchAnimators = false;

        private SerializedObject _serializedObject;

        private static void FetchAnimators()
        {
            if (_animators.Count > 0 && !ShouldFetchAnimators) return;

            ShouldFetchAnimators = false;

            _animators.Clear();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AnimatorController)}");
            foreach (var assetGuid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                var animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                if (animator == null) continue;
                _animators.Add(animator, GetAnimatorReferencesAsset(animator));
            }
        }

        private static AnimatorStateReferences GetAnimatorReferencesAsset(AnimatorController animator)
        {
            // 1. Safety check
            if (animator == null)
                return null;

            // 2. Get the physical path of the AnimatorController in the project
            string path = AssetDatabase.GetAssetPath(animator);
            if (string.IsNullOrEmpty(path))
                return null;

            // 3. Load the main asset AND all of its hidden/nested sub-assets
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            // 4. Loop through the loaded assets to find your specific type
            foreach (Object asset in allAssets)
            {
                if (asset is AnimatorStateReferences referenceAsset)
                {
                    // Found it! Return the sub-asset
                    return referenceAsset;
                }
            }

            // It doesn't exist under this AnimatorController
            return null;
        }

        private static AnimatorStateReferences CreateAnimatorReferencesAsset(AnimatorController animator)
        {
            // 1. Safety check to ensure the AnimatorController exists
            if (animator == null)
            {
                Debug.LogWarning("Animator is null. Cannot create sub-asset.");
                return null;
            }

            // 2. Ensure the Animator is actually saved on the disk
            string path = AssetDatabase.GetAssetPath(animator);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("AnimatorController is not saved to disk yet!");
                return null;
            }

            // 3. Create a brand new instance of your ScriptableObject in memory
            AnimatorStateReferences newReferenceAsset = ScriptableObject.CreateInstance<AnimatorStateReferences>();
            newReferenceAsset.name = "State References"; // This is the name that will show up in the Project window
            newReferenceAsset.animator = animator;

            // 4. Attach it as a sub-asset to the main AnimatorController
            AssetDatabase.AddObjectToAsset(newReferenceAsset, animator);

            // 5. Force Unity to save the changes to the disk immediately
            AssetDatabase.SaveAssets();

            return newReferenceAsset;
        }

        private static UnityEditor.IMGUI.Controls.AdvancedDropdownState _dropdownState = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FetchAnimators();

            _serializedObject = property.serializedObject;
            var guidProperty = property.FindPropertyRelative("_identifier");
            var referencesProperty = property.FindPropertyRelative("_references");

            var references = (AnimatorStateReferences)referencesProperty.objectReferenceValue;

            AnimatorController animatorController = null;
            SerializedAnimatorState state = default;

            if (references)
            {
                animatorController = references.animator;
                if (references.states.ContainsKey(guidProperty.longValue))
                {
                    state = references.states[guidProperty.longValue];
                }
            }
            
            string name = string.IsNullOrEmpty(state.name) ? "" : state.name;

            EditorGUI.BeginProperty(position, label, property);

            // Layout setup
            Rect textRect = new Rect(position.x, position.y, position.width - 44, position.height);
            Rect dropdownRect = new Rect(position.x + position.width - 42, position.y, 20, position.height);
            Rect assetRect = new Rect(position.x + position.width - 20, position.y, 20, position.height);

            // Text field
            EditorGUI.BeginDisabledGroup(true);
            string text = name;
            string shortText = name;

            if (animatorController && !string.IsNullOrEmpty(name))
            {
                if (animatorController.layers.Length > state.layer)
                {
                    text = $"{animatorController.layers[state.layer].name}/{text}";
                }
                text = $"{animatorController.name}/{text}";

                shortText = $"{animatorController.name}/{name.Split('/').Last()}";
            }

            bool isHovering = Event.current.type == EventType.Repaint && textRect.Contains(Event.current.mousePosition);

            if (isHovering)
            {
                EditorGUI.TextField(textRect, label, text);
            }
            else
            {
                EditorGUI.TextField(textRect, label, shortText);
            }

            EditorGUI.EndDisabledGroup();

            // Dropdown button
            if (GUI.Button(dropdownRect, "▾"))
            {
                var dropdown = new AnimatorStateAdvancedDropdown(_dropdownState, _animators, (animator, selectedState, layer) =>
                {
                    if (!_animators[animator]) _animators[animator] = CreateAnimatorReferencesAsset(animator);

                    var references = _animators[animator];

                    long guid = 0;

                    foreach (var pair in references.states)
                    {
                        if (pair.value.state != selectedState) continue;
                        guid = pair.key;
                        break;
                    }

                    if (guid == 0) guid = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);

                    references.states[guid] = new()
                    {
                        state = selectedState,
                        name = selectedState.name,
                        layer = layer,
                        duration = selectedState.motion != null ? selectedState.motion.averageDuration / selectedState.speed : 0,
                    };

                    EditorUtility.SetDirty(references);
                    AssetDatabase.SaveAssetIfDirty(references);

                    guidProperty.longValue = guid;
                    referencesProperty.objectReferenceValue = references;

                    _serializedObject.ApplyModifiedProperties();
                });

                dropdown.Show(dropdownRect);
            }

            // Asset select button
            if (GUI.Button(assetRect, "▣"))
            {
                if (animatorController)
                {
                    Selection.activeObject = animatorController;
                }
            }

            EditorGUI.EndProperty();
        }

    }
}