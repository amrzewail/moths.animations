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

        public float duration => 0;

        public IAnimationState[] combine => null;

        public StopAnimationState(int layer)
        {
            this.layer = layer;
        }
    }
    public struct EmptyAnimationState : IAnimationState
    {
        public int layer { get; }


        public string animID => "__EmptyID__";
        public string stateName => "Empty";

        public float duration => 0;

        public IAnimationState[] combine => null;

        public EmptyAnimationState(int layer)
        {
            this.layer = layer;
        }
    }
}