using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations.Playables
{
    public struct BasicAnimationCreator<TAnimation> : IPlayableCreator where TAnimation : IAnimation
    {
        private TAnimation _animation;

        public BasicAnimationCreator(TAnimation animation)
        {
            _animation = animation;
        }

        public Playable Create(PlayableGraph graph)
        {
            var playable = AnimationClipPlayable.Create(graph, _animation.clip);
            playable.SetApplyFootIK(_animation.applyIK);
            playable.SetApplyPlayableIK(_animation.applyIK);
            return playable;
        }

        public float GetDuration()
        {
            return _animation.clip.length * _animation.speed;
        }

        public bool IsLoop() => _animation.clip.isLooping || _animation.clip.wrapMode == WrapMode.Loop || _animation.clip.wrapMode == WrapMode.PingPong;
    }
}