using System.Runtime.InteropServices;
using UnityEngine;

namespace Anima
{
    public struct UAnimationState : IAnimationState
    {
        public int layer { get; set; }

        public string animID { get; set; }

        public string stateName { get; set; }

        public float duration { get; set; }

        public IAnimationState[] combine { get; set; }

        public static UAnimationState ConstructFrom(IAnimationState state)
        {
            var ustate = new UAnimationState();
            ustate.layer = state.layer;
            ustate.animID = state.animID;
            ustate.stateName = state.stateName;
            ustate.duration = state.duration;
            ustate.combine = state.combine;
            return ustate;
        }
    }
}