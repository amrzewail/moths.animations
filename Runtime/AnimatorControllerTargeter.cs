using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;

#endif

namespace Anima.Internal
{
    [CreateAssetMenu(menuName = "Animations/Animator Controller Targeter")]
    public class AnimatorControllerTargeter : ScriptableObject
    {
#if UNITY_EDITOR

        [System.Serializable]
        private struct StateReference
        {
            public AnimatorState target;
            public AnimationState state;
        }

        [SerializeField] AnimatorController _animatorController;
        [SerializeField] bool _refresh;
        [SerializeField] List<StateReference> _references = new List<StateReference>();


        private void OnValidate()
        {
            _refresh = false;

            if (!_animatorController) return;
            if (_animatorController.layers == null) return;

            HashSet<AnimatorState> existingStates = new HashSet<AnimatorState>(); 
            for (int i = 0; i < _animatorController.layers.Length; i++)
            {
                RefreshStates(i, _animatorController.layers[i].stateMachine, existingStates);
            }

            for (int i = 0; i < _references.Count; i++)
            {
                if (existingStates.Contains(_references[i].target)) continue;
                AssetDatabase.RemoveObjectFromAsset(_references[i].state);
                EditorUtility.SetDirty(this);
                _references.RemoveAt(i);
                i--;
            }

            string assetPath = AssetDatabase.GetAssetPath(this);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        private AnimationState FindState(AnimatorState state)
        {
            for (int i = 0; i < _references.Count; i++) 
            {
                if (_references[i].target == state) return _references[i].state;
            }
            return null;
        }

        private void RefreshStates(int layer, AnimatorStateMachine stateMachine, HashSet<AnimatorState> existingStates)
        {
            var states = stateMachine.states.ToList();

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                int identifier = state.state.nameHash;
                existingStates.Add(state.state);

                var animationState = FindState(state.state);
                if (!animationState)
                {
                    animationState = ScriptableObject.CreateInstance<AnimationState>();
                    AssetDatabase.AddObjectToAsset(animationState, this);
                    _references.Add(new StateReference { target = state.state, state = animationState });
                    EditorUtility.SetDirty(this);
                }

                animationState.name = state.state.name;

                FieldInfo animatorControllerField = typeof(AnimationState).GetField("_animatorController", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo layerField = typeof(AnimationState).GetField("_layer", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo stateNameField = typeof(AnimationState).GetField("_stateName", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo durationField = typeof(AnimationState).GetField("_duration", BindingFlags.NonPublic | BindingFlags.Instance);

                animatorControllerField.SetValue(animationState, _animatorController);
                layerField.SetValue(animationState, layer);
                stateNameField.SetValue(animationState, state.state.name);
                durationField.SetValue(animationState, AnimationState.CalculateDuration(state.state));

                EditorUtility.SetDirty(animationState);
            }

            var machines = stateMachine.stateMachines;


            for (int i = 0; i < machines.Length; i++)
            {
                RefreshStates(layer, machines[i].stateMachine, existingStates);
            }

        }
#endif
    }
}