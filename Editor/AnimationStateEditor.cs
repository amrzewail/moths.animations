using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Anima.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(AnimationState))]
    [CanEditMultipleObjects]
    public class AnimationStateEditor : Editor
    {
        private class LinkedState
        {
            public string name;
            public LinkedState parent;
        }

        SerializedProperty _animatorProperty;
        SerializedProperty _layerProperty;
        SerializedProperty _animIDProperty;
        SerializedProperty _stateNameProperty;
        SerializedProperty _durationProperty;


        private void OnEnable()
        {
            _animatorProperty = serializedObject.FindProperty("_animatorController");
            _layerProperty = serializedObject.FindProperty("_layer");
            _animIDProperty = serializedObject.FindProperty("_animID");
            _stateNameProperty = serializedObject.FindProperty("_stateName");
            _durationProperty = serializedObject.FindProperty("_duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            AnimationState instance = (AnimationState)target;

            AnimatorController animator = instance.animatorController;

            EditorGUILayout.PropertyField(_animatorProperty);

            if (animator)
            {

                var layer = Mathf.Clamp(_layerProperty.intValue, 0, animator.layers.Length - 1);
                var stateMachine = animator.layers[layer].stateMachine;
                _layerProperty.intValue = Mathf.Clamp(_layerProperty.intValue, 0, animator.layers.Length);
                EditorGUILayout.PropertyField(_layerProperty);

                EditorGUILayout.PropertyField(_animIDProperty);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("State name");


                ChildAnimatorState selectedState;
                string statePath = "";

                if (!string.IsNullOrEmpty(_stateNameProperty.stringValue))
                {
                    if (FindState(stateMachine, _stateNameProperty.stringValue, new List<string>(), out selectedState, out statePath))
                    {
                        _durationProperty.floatValue = AnimationState.CalculateDuration(selectedState.state);
                    }
                }


                if (EditorGUILayout.DropdownButton(new GUIContent(statePath), FocusType.Passive, GUILayout.ExpandWidth(true)))
                {
                    GenericMenu menu = new GenericMenu();
                    AddStateMachineToMenu(menu, stateMachine, new List<string>());

                    menu.ShowAsContext();
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Duration");
                EditorGUILayout.LabelField(_durationProperty.floatValue.ToString());
                EditorGUILayout.EndHorizontal();

            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddStateMachineToMenu(GenericMenu menu, AnimatorStateMachine stateMachine, List<string> stack)
        {
            var states = stateMachine.states.ToList();

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                string statePath = (stack.Count > 0 ? string.Join('/', stack) + "/" : "") + state.state.name;
                menu.AddItem(new GUIContent(statePath), _stateNameProperty.stringValue.Equals(state.state.name), () =>
                {
                    _stateNameProperty.stringValue = state.state.name;
                    serializedObject.ApplyModifiedProperties();
                });
            }

            var machines = stateMachine.stateMachines;


            for (int i = 0; i < machines.Length; i++)
            {
                stack.Add(machines[i].stateMachine.name);

                AddStateMachineToMenu(menu, machines[i].stateMachine, stack);

                stack.RemoveAt(stack.Count - 1);
            }

        }

        private bool FindState(AnimatorStateMachine stateMachine, string name, List<string> stack, out ChildAnimatorState state, out string statePath)
        {
            var states = stateMachine.states;

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state.name.Equals(name))
                {
                    state = states[i];
                    statePath = string.Join("/", stack) + "/" + state.state.name;
                    return true;
                }
            }

            var machines = stateMachine.stateMachines;

            for (int i = 0; i < machines.Length; i++)
            {
                stack.Add(machines[i].stateMachine.name);
                if (FindState(machines[i].stateMachine, name, stack, out state, out statePath)) return true;
                stack.RemoveAt(stack.Count - 1);
            }

            state = default;
            statePath = "";
            return false;
        }
    }
}