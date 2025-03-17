using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anima
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
}