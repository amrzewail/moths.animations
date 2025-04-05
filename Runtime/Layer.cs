using Moths.Animations.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations
{
    public struct Layer
    {
        private struct Playable
        {
            public AnimationClipPlayable animation;
        }

        private struct QueuedAnimation
        {
            public IAnimationState state;
            public AnimationPlayInfo info;
        }

        private PlayableGraph _graph;
        private AnimationLayerMixerPlayable _mixer;

        private IndexedDictionary<IAnimationState, Playable> _playables;

        public IAnimationState CurrentState => _queue.Current.state;
        public AnimationPlayInfo PlayInfo => _queue.Current.info;

        public bool IsPlaying => CurrentState != null;

        public bool IsFinished => NormalizedTime >= 1;
        public AnimationLayerMixerPlayable Mixer => _mixer;

        private Collections.Queue<QueuedAnimation> _queue;

        public float Time
        {
            get
            {
                if (!IsPlaying) return 0;
                return (float)_playables[CurrentState].animation.GetTime();
            }
        }

        public float NormalizedTime
        {
            get
            {
                if (!IsPlaying) return 0;
                if (CurrentState.clip.isLooping)
                {
                    return (Time / CurrentState.clip.length) % 1;
                }
                else
                {
                    return Mathf.Clamp01(Time / CurrentState.clip.length);
                }
            }
        }

        public Layer(PlayableGraph graph)
        {
            this = default;

            _queue = new(8);

            _graph = graph;

            _mixer = AnimationLayerMixerPlayable.Create(_graph, 16);
            _playables = new(16);

            for (int i = 0; i < _playables.Count; i++) _mixer.SetLayerAdditive((uint)i, false);
        }

        public void Queue(IAnimationState state, AnimationPlayInfo playInfo)
        {
            _queue.Add(new QueuedAnimation
            {
                state = state,
                info = playInfo
            });
            if (_queue.Count == 1)
            {
                CrossFade(CurrentState, PlayInfo);
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        public void CrossFade(IAnimationState animation, AnimationPlayInfo playInfo)
        {
            if (animation == null) return;

            Playable playable;

            if (!_playables.Contains(animation))
            {
                playable = new Playable
                {
                    animation = AnimationClipPlayable.Create(_graph, animation.clip),
                };

                _graph.Connect(playable.animation, 0, _mixer, _playables.Count);
                _playables[animation] = playable;
            }
            else
            {
                playable = _playables[animation];
            }

            int mixerIndex = _playables.IndexOf(animation);
            float weight = _mixer.GetInputWeight(mixerIndex);

            _mixer.SetInputWeight(mixerIndex, 0);

            playable.animation.SetTime(0);
            playable.animation.SetSpeed(animation.speed * playInfo.speed);

            _graph.Evaluate();

            _mixer.SetInputWeight(mixerIndex, weight);

            _playables[animation] = playable;

            int mixerInputCount = _mixer.GetInputCount();
            if (_playables.Count > mixerInputCount)
            {
                _mixer.SetInputCount(mixerInputCount = mixerInputCount * 2);
                for (int i = 0; i < mixerInputCount; i++) _mixer.SetLayerAdditive((uint)i, false);
            }

            _mixer.SetSpeed(1);
        }

        public void Stop()
        {
            if (!IsPlaying) return;

            _mixer.SetSpeed(0);
            _queue.Clear();
        }

        public void Update(float deltaTime)
        {
            if (CurrentState == null) return;

            if (_queue.HasNext())
            {
                var info = _queue.PeekNext().info;
                if (CurrentState.clip.length - Time <= info.blendTime)
                {
                    _queue.Next();
                    CrossFade(CurrentState, PlayInfo);
                    return;
                }
            }

            var playable = _playables[CurrentState];

            int currentIndex = _playables.IndexOf(CurrentState);
            int count = _playables.Count;

            float t = 1;
            if (!Mathf.Approximately(PlayInfo.blendTime, 0))
            {
                t = (float)playable.animation.GetTime() / PlayInfo.blendTime;
                t = Mathf.Pow(t, 2);
            }

            bool maximizeCurrent = _playables.Count == 1;

            for (int i = count - 1; i >= 0; i--)
            {
                float weight = _mixer.GetInputWeight(i);

                if (i > currentIndex)
                {
                    if (weight < 0.000001f) continue;
                    maximizeCurrent = true;
                    weight = Mathf.Lerp(weight, 0, t);
                }
                else if (i == currentIndex)
                {
                    if (maximizeCurrent)
                    {
                        weight = 1;
                    }
                    else
                    {
                        weight = Mathf.Lerp(weight, 1, t);
                    }
                }
                else
                {
                    weight = Mathf.Lerp(weight, 0, t - 1);
                }

                _mixer.SetInputWeight(i, Mathf.Clamp01(weight));
            }
        }

    }
}