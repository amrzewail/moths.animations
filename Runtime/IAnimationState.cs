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

        AnimationClip clip { get; }

        float speed { get; }

        float duration { get; }

        AvatarMask mask { get; }

        IAnimationState[] combine { get; }
    }
}