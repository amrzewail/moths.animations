using Moths.Animations.Collections;
using Moths.Animations.Playables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public interface IAnimation
    {
        IPlayableCreator playable { get; }

        AnimLayer layer { get; }
        float duration => speed > 0 ? clip.length / speed : 0;

        string animID { get; }
        AnimationClip clip { get; }
        float speed { get; }

        bool IsNull() => !IsValid();
        bool IsValid() => clip;
    }
}