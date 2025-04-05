using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moths.Animations.Playables;
using Moths.Animations.Collections;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

using Moths.Fields;
namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation State", menuName = "Moths/Animations/Animation State")]
    public class AnimationState : ScriptableObject, IAnimation
    {
#region LEGACY
        [SerializeField] int _layer;
        [SerializeField] string _stateName;
        [SerializeField] float _duration;

#if UNITY_EDITOR
        [SerializeField] AnimatorController _animatorController;
        public AnimatorController animatorController => _animatorController;
#endif
#endregion

        [SerializeField] StringReference _animID;
        [SerializeField] AnimationClip _clip;
        [SerializeField] float _speed = 1;
        [SerializeField] AvatarMask _mask;


        public string animID => _animID;
        public AnimationClip clip => _clip;
        public float speed => _speed;

        public AvatarMask mask => _mask;

        public IAnimation[] combine => null;

        public IPlayableCreator playable => null;

        public AnimLayer layer => throw new System.NotImplementedException();

#if UNITY_EDITOR
        public static float CalculateDuration(AnimatorState state)
        {
            return state.motion != null ? state.motion.averageDuration / state.speed : 0;
        }
#endif
    }
}