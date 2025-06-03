using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Fields;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

using Moths.Attributes;
using Moths.Collections;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Moths.Animations
{
    [System.Serializable]
    public struct UAnimation : IAnimation
    {
        public Unique uniqueId { get => _uniqueId; set => _uniqueId = value; }
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

        public AnimationClip clip { get => _clip; set => _clip = value; }
        public AvatarMask mask { get => _mask; set => _mask = value; }
        public float speed { get => _speed; set => _speed = value; }
        public bool loop { get => _loop || (_playable != null && _playable.IsLoop()) || (_clip && _clip.isLooping); set => _loop = value; }
        public bool applyIK => _applyIK;
        public float duration => (float)_playable.GetDuration();

        private IPlayableCreator _playable;
        private string _fallbackAnimID;

        [SerializeField] Unique _uniqueId;
        [SerializeField] StringReference _animID;
        [SerializeField] AnimationClip _clip;
        [SerializeField] AvatarMask _mask;
        [SerializeField] float _speed;
        [SerializeField] bool _loop;
        [SerializeField] bool _applyIK;

        public IPlayableCreator playable
        {
            get => _playable ?? (_playable = new BasicAnimationCreator<UAnimation>(this));
            set => _playable = value;
        }

        public static UAnimation ConstructFrom<TAnimation>(TAnimation state) where TAnimation : IAnimation
        {
            var ustate = new UAnimation();
            ustate.uniqueId = state.uniqueId;
            ustate.animID = state.animID;
            ustate.clip = state.clip;
            ustate._mask = state.layer.Mask;
            ustate.speed = state.speed;
            ustate.loop = state.loop;
            ustate.playable = state.playable;
            return ustate;
        }

        public override bool Equals(object obj)
        {
            return obj is UAnimation anim && anim.uniqueId == uniqueId && anim.clip == clip && anim.mask == mask && anim.animID == animID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(uniqueId, clip, animID, mask);
        }

        public bool IsNull() => !IsValid();
        public bool IsValid() => uniqueId != Unique.Empty && _playable != null;
    }
}