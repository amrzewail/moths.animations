using System.Runtime.InteropServices;
using UnityEngine;

namespace Moths.Animations
{
    public struct UAnimationState : IAnimationState
    {
        public int layer { get; set; }

        public string animID { get; set; }

        public string stateName { get; set; }

        public float duration { get; set; }
        public float speed { get; set; }
        public AvatarMask mask { get; set; }

        public IAnimationState[] combine { get; set; }

        public AnimationClip clip { get; set; }

        public static UAnimationState ConstructFrom(IAnimationState state)
        {
            var ustate = new UAnimationState();
            ustate.layer = state.layer;
            ustate.animID = state.animID;
            ustate.stateName = state.stateName;
            ustate.clip = state.clip;
            ustate.duration = state.duration;
            ustate.speed = state.speed;
            ustate.mask = state.mask;
            ustate.combine = state.combine;
            return ustate;
        }
    }
}