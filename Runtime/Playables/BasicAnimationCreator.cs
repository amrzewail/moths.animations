using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations.Playables
{
    public struct BasicAnimationCreator : IPlayableCreator
    {
        private UAnimation _animation;

        public BasicAnimationCreator(IAnimation animation)
        {
            _animation = UAnimation.ConstructFrom(animation);
        }

        public Playable Create(PlayableGraph graph)
        {
            return AnimationClipPlayable.Create(graph, _animation.clip);

        }
    }
}