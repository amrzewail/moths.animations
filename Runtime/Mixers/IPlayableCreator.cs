using UnityEngine;
using UnityEngine.Playables;

namespace Moths.Animations.Playables
{
    public interface IPlayableCreator
    {
        Playable Create(PlayableGraph graph);

        float GetDuration();

        bool IsLoop();
    }
}