using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

using Moths.Fields;
namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation State", menuName = "Animations/Animation State")]
    public class AnimationState : ScriptableObject, IAnimationState
    {
        [SerializeField] int _layer;

#if UNITY_EDITOR
        [SerializeField] AnimatorController _animatorController;
#endif

        [SerializeField] StringReference _animID;
        [SerializeField] string _stateName;
        [SerializeField] float _duration;

#if UNITY_EDITOR
        public AnimatorController animatorController => _animatorController;
#endif

        public int layer => _layer;

        public string animID => _animID;
        public string stateName => _stateName;

        public float duration => Mathf.Abs(_duration);

        public IAnimationState[] combine => null;

#if UNITY_EDITOR
        public static float CalculateDuration(AnimatorState state)
        {
            return state.motion != null ? state.motion.averageDuration / state.speed : 0;
        }
#endif
    }
}