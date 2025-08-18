using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Moths.Fields;
using Moths.Animations.Attributes;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Combine Animation State", menuName = "Moths/Animations/Combine Animation State")]
    public class CombineAnimationState : ScriptableObject, IAnimationState
    {

        [SerializeField] StringReference _animID;
        [SerializeField][RequireInterface(typeof(IAnimationState))] Object[] _combine;

        private IAnimationState[] _cachedStates;

        public IAnimationState[] combine
        {
            get
            {
                if (_cachedStates == null || _cachedStates.Length != _combine.Length)
                {
                    _cachedStates = _combine.Cast<IAnimationState>().ToArray();
                }

                return _cachedStates;
            }
        }

        public int layer => ((IAnimationState)_combine[0]).layer;

        public string animID => _animID;
        public string stateName => "";

        public float duration => ((IAnimationState)_combine[0]).duration;
    }
}