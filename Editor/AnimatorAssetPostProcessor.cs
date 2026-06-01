using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Moths.Animations
{
    public class AnimatorAssetPostProcessor : AssetPostprocessor
    {
        // This Unity callback fires automatically whenever ANY asset is saved/imported/changed
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                // 1. Check if the asset that just got updated is an Animator Controller
                if (path.EndsWith(".controller"))
                {
                    AnimatorController animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                    if (animator != null)
                    {
                        AnimatorStateDrawer.ShouldFetchAnimators = true;

                        // 2. Find the sub-asset inside this specific Animator
                        AnimatorStateReferences referencesAsset = GetAnimatorReferencesAsset(path);

                        // 3. If it exists, do your stuff!
                        if (referencesAsset != null)
                        {
                            referencesAsset.animator = animator;

                            for (int i = 0; i < animator.layers.Length; i++) UpdateStateReferences(referencesAsset, animator.layers[i].stateMachine, i, new());

                            EditorUtility.SetDirty(referencesAsset);
                        }
                    }
                }
            }
        }

        private static void UpdateStateReferences(AnimatorStateReferences references, AnimatorStateMachine stateMachine, int layer, List<string> stack)
        {
            var states = stateMachine.states.ToList();

            string layerName = references.animator.layers[layer].name;

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                string statePath = (stack.Count > 0 ? string.Join('/', stack) + "/" : "") + state.state.name;
                foreach (var pair in references.states)
                {
                    if (pair.value.state != state.state) continue;

                    references.states[pair.key] = new()
                    {
                        state = state.state,
                        name = statePath,
                        layer = layer,
                        duration = state.state.motion != null ? state.state.motion.averageDuration / state.state.speed : 0,
                    };

                    break;
                }
            }

            var machines = stateMachine.stateMachines;


            for (int i = 0; i < machines.Length; i++)
            {
                stack.Add(machines[i].stateMachine.name);
                UpdateStateReferences(references, machines[i].stateMachine, layer, stack);
                stack.RemoveAt(stack.Count - 1);
            }
        }

        // Helper method to dig into the file path and find your ScriptableObject
        private static AnimatorStateReferences GetAnimatorReferencesAsset(string path)
        {
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in allAssets)
            {
                if (asset is AnimatorStateReferences referenceAsset)
                {
                    return referenceAsset; // Found it
                }
            }

            return null; // Doesn't exist
        }
    }
}