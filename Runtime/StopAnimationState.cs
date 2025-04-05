using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public struct StopAnimationState : IAnimationState
    {
        public int layer { get; }


        public string animID => "__StopID__";
        public string stateName => "Stop";
        public AnimationClip clip => null;
        public float duration => 0;
        public float speed => 0;
        public AvatarMask mask => null;

        public IAnimationState[] combine => null;

        public StopAnimationState(int layer)
        {
            this.layer = layer;
        }
    }
}