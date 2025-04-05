using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Fields;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Moths.Animations
{
    [System.Serializable]
    public struct UAnimation : IAnimation
    {
        public AnimLayer layer => new AnimLayer(mask);
        public string animID 
        { 
            get => _animID != null ? _animID : _fallbackAnimID;
            set
            {
                _fallbackAnimID = value;
                if (_animID == null) return;
                _animID.Value = value;
            }
        }

        public AvatarMask mask { get => _mask; set => _mask = value; }
        public AnimationClip clip { get => _clip; set => _clip = value; }
        public float speed { get => _speed; set => _speed = value; }
        public IPlayableCreator playable { get; set; }

        private string _fallbackAnimID;

        [SerializeField] StringReference _animID;
        [SerializeField] AvatarMask _mask;
        [SerializeField] AnimationClip _clip;
        [SerializeField] float _speed;

        public static UAnimation ConstructFrom(IAnimation state)
        {
            var ustate = new UAnimation();
            ustate.animID = state.animID;
            ustate.clip = state.clip;
            ustate.speed = state.speed;
            ustate._mask = state.layer.Mask;
            ustate.playable = state.playable;
            return ustate;
        }

        public override bool Equals(object obj)
        {
            return obj is UAnimation anim && anim.clip == clip && anim.mask == mask && anim.animID == animID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(animID, mask, clip);
        }

        public bool IsNull() => !IsValid();
        public bool IsValid() => clip;
    }
}