using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Moths.Fields;
using Moths.Animations.Attributes;
using Moths.Animations.Playables;
using Moths.Animations.Collections;
using Moths.Collections;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Combine Animation State", menuName = "Moths/Animations/Combine Animation State")]
    public class CombineAnimationState : ScriptableObject
    {

        [SerializeField] StringReference _animID;
        [SerializeField][RequireInterface(typeof(IAnimation))] Object[] _combine;

        private IAnimation[] _cachedStates;

        public IAnimation[] combine
        {
            get
            {
                if (_cachedStates == null || _cachedStates.Length != _combine.Length)
                {
                    _cachedStates = _combine.Cast<IAnimation>().ToArray();
                }

                return _cachedStates;
            }
        }

        public string animID => _animID;
        public string stateName => "";

        public AnimationClip clip => null;

        public float speed => 1;

        public AvatarMask mask => null;

        public IPlayableCreator playable => throw new System.NotImplementedException();

        public AnimLayer layer => throw new System.NotImplementedException();

        public float length => throw new System.NotImplementedException();

        public Unique uniqueId => throw new System.NotImplementedException();

        public bool loop => throw new System.NotImplementedException();

        public bool applyIK => throw new System.NotImplementedException();

        public bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}