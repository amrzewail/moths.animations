using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Moths.Animations
{
    public class AnimatorStateAdvancedDropdown : AdvancedDropdown
    {
        private readonly Action<AnimatorController, UnityEditor.Animations.AnimatorState, int> _onSelected;
        private readonly Dictionary<AnimatorController, AnimatorStateReferences> _animators;

        public class AnimatorStateItem : AdvancedDropdownItem
        {
            public AnimatorController Animator { get; }
            public UnityEditor.Animations.AnimatorState State { get; }
            public int Layer { get; }

            public AnimatorStateItem(string name, AnimatorController animator, UnityEditor.Animations.AnimatorState state, int layer) : base(name)
            {
                Animator = animator;
                State = state;
                Layer = layer;
            }
        }

        public AnimatorStateAdvancedDropdown(AdvancedDropdownState state, Dictionary<AnimatorController, AnimatorStateReferences> animators, Action<AnimatorController, UnityEditor.Animations.AnimatorState, int> onSelected) : base(state)
        {
            _animators = animators;
            _onSelected = onSelected;
            minimumSize = new Vector2(300, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Animator States");

            foreach (var pair in _animators)
            {
                var animator = pair.Key;
                var animatorItem = new AdvancedDropdownItem(animator.name)
                {
                    icon = (Texture2D)EditorGUIUtility.IconContent("AnimatorController Icon").image
                };
                root.AddChild(animatorItem);

                for (int i = 0; i < animator.layers.Length; i++)
                {
                    var layer = animator.layers[i];
                    var layerItem = new AdvancedDropdownItem(layer.name)
                    {
                        icon = (Texture2D)EditorGUIUtility.IconContent("LayerMask Icon").image
                    };
                    animatorItem.AddChild(layerItem);

                    AddStateMachineToItem(layerItem, layer.stateMachine, animator, i);
                }
            }

            return root;
        }

        private void AddStateMachineToItem(AdvancedDropdownItem parent, AnimatorStateMachine stateMachine, AnimatorController animator, int layer)
        {
            foreach (var childState in stateMachine.states)
            {
                var stateItem = new AnimatorStateItem(childState.state.name, animator, childState.state, layer)
                {
                    icon = (Texture2D)EditorGUIUtility.IconContent("AnimatorState Icon").image
                };
                parent.AddChild(stateItem);
            }

            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                var subMachineItem = new AdvancedDropdownItem(childStateMachine.stateMachine.name)
                {
                    icon = (Texture2D)EditorGUIUtility.IconContent("AnimatorStateMachine Icon").image
                };
                parent.AddChild(subMachineItem);
                AddStateMachineToItem(subMachineItem, childStateMachine.stateMachine, animator, layer);
            }
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is AnimatorStateItem stateItem)
            {
                _onSelected?.Invoke(stateItem.Animator, stateItem.State, stateItem.Layer);
            }
        }
    }
}
