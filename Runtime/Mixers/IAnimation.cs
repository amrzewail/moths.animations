using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Moths.Animations
{
    public interface IAnimation
    {
        Unique uniqueId { get; }
        AnimLayer layer { get; }
        AnimationClip clip { get; }
        string animID { get; }
        float speed { get; }
        bool loop { get; }
        bool applyIK { get; }
        bool IsNull() => !IsValid();
        bool IsValid();
        IPlayableCreator playable { get; }
    }
}