using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations.Playables
{
    public struct MixAnimationCreator<TAnimation> : IPlayableCreator where TAnimation : IAnimation
    {
        private TAnimation[] _animations;

        public MixAnimationCreator(TAnimation[] animations)
        {
            _animations = animations;
        }

        public Playable Create(PlayableGraph graph)
        {
            var mixer = AnimationLayerMixerPlayable.Create(graph, _animations.Length + 1);

            int index = 0;
            for (int i = 0; i < _animations.Length; i++)
            {
                var playable = new BasicAnimationCreator(_animations[i]);
                graph.Connect(playable.Create(graph), 0, mixer, index);
                mixer.SetInputWeight(index, 1);
                if (_animations[i].layer.Mask) mixer.SetLayerMaskFromAvatarMask((uint)index, _animations[i].layer.Mask);
                index++;
            }

            return mixer;

        }
    }
}