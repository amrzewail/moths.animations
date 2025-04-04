using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public interface IAnimationState
    {
        int layer { get; }

        string animID { get; }
        string stateName { get; }

        float duration { get; }

        IAnimationState[] combine { get; }
    }
}