using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
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
        private SerializedProperty _referencesProperty;
        private SerializedProperty _guidProperty;

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

        private void AddStateMachineToMenu(GenericMenu menu, AnimatorStateMachine stateMachine, AnimatorController animator, int layer, List<string> stack)
        {
            var states = stateMachine.states.ToList();

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                string statePath = animator.name + '/' + animator.layers[layer].name + '/' + (stack.Count > 0 ? string.Join('/', stack) + "/" : "") + state.state.name;
                menu.AddItem(new GUIContent(statePath), false, () =>
                {
                    if (!_animators[animator]) _animators[animator] = CreateAnimatorReferencesAsset(animator);

                    var references = _animators[animator];

                    string guid = string.Empty;

                    foreach (var pair in references.states)
                    {
                        if (pair.value.state != state.state) continue;
                        guid = pair.key;
                        break;
                    }

                    if (string.IsNullOrEmpty(guid)) guid = Guid.NewGuid().ToString();

                    references.states[guid] = new()
                    {
                        state = state.state,
                        name = state.state.name,
                        layer = layer,
                        duration = state.state.motion != null ? state.state.motion.averageDuration / state.state.speed : 0,
                    };

                    EditorUtility.SetDirty(references);
                    AssetDatabase.SaveAssetIfDirty(references);

                    _guidProperty.stringValue = guid;
                    _referencesProperty.objectReferenceValue = references;

                    _serializedObject.ApplyModifiedProperties();

                });
            }

            var machines = stateMachine.stateMachines;


            for (int i = 0; i < machines.Length; i++)
            {
                stack.Add(machines[i].stateMachine.name);

                AddStateMachineToMenu(menu, machines[i].stateMachine, animator, layer, stack);

                stack.RemoveAt(stack.Count - 1);
            }

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FetchAnimators();

            _serializedObject = property.serializedObject;
            _guidProperty = property.FindPropertyRelative("_guid");
            _referencesProperty = property.FindPropertyRelative("_references");

            var references = (AnimatorStateReferences)_referencesProperty.objectReferenceValue;

            AnimatorController animatorController = null;
            SerializedAnimatorState state = default;

            if (references)
            {
                animatorController = references.animator;
                state = references.states[_guidProperty.stringValue];
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

            if (animatorController)
            {
                if (animatorController.layers.Length > state.layer)
                {
                    text = $"{animatorController.layers[state.layer].name}/{text}";
                }
                text = $"{animatorController.name}/{text}";
            }
            bool isHovering = Event.current.type == EventType.Repaint && textRect.Contains(Event.current.mousePosition);

            if (isHovering)
            {
                EditorGUI.TextField(textRect, label, _guidProperty.stringValue);
            }
            else
            {
                EditorGUI.TextField(textRect, label, text);
            }

            EditorGUI.EndDisabledGroup();

            // Dropdown button
            if (GUI.Button(dropdownRect, "▾"))
            {
                GenericMenu menu = new GenericMenu();

                if (_animators.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No AnimatorControllers found."));
                }
                else
                {
                    foreach(var pair in _animators)
                    {
                        var animator = pair.Key;
                        for (int i = 0; i < animator.layers.Length; i++)
                        {
                            AddStateMachineToMenu(menu, animator.layers[i].stateMachine, animator, i, new List<string>());
                        }
                    }

                }
                menu.DropDown(dropdownRect);
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