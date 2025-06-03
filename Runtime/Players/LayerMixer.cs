using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Animations.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations
{
    public struct LayerMixer
    {
        private struct QueuedAnimation
        {
            public UAnimation animation;
            public AnimationPlayInfo info;
        }

        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        private Playable _output;

        private IndexedDictionary<UAnimation, Playable> _playables;

        public UAnimation CurrentAnimation => _queue.Current.animation;
        public AnimationPlayInfo PlayInfo => _queue.Current.info;

        public bool IsPlaying => !CurrentAnimation.IsNull() && !IsFinished;

        public bool IsFinished => NormalizedTime >= 1 && (CurrentAnimation.IsValid() && !CurrentAnimation.loop);
        public Playable Mixer => _output;

        private Collections.Queue<QueuedAnimation> _queue;

        private float _elapsedTime;

        public float Time
        {
            get
            {
                if (CurrentAnimation.IsNull()) return 0;
                return _elapsedTime * PlayInfo.speed;
            }
        }

        public float NormalizedTime
        {
            get
            {
                if (CurrentAnimation.IsNull()) return 0;
                if (CurrentAnimation.loop)
                {
                    return (Time / CurrentAnimation.duration) % 1;
                }
                else
                {
                    return Mathf.Clamp01(Time / CurrentAnimation.duration);
                }
            }
            set
            {
                if (CurrentAnimation.IsNull()) return;
                var playable = _playables[CurrentAnimation];
                playable.SetTime((value * CurrentAnimation.duration - UnityEngine.Time.deltaTime));
                playable.SetTime((value * CurrentAnimation.duration));
                _elapsedTime = 0;
            }
        }

        public LayerMixer(PlayableGraph graph)
        {
            this = default;

            _queue = new(8);

            _graph = graph;

            _mixer = AnimationMixerPlayable.Create(_graph, 16);
            _playables = new(16);

            _output = _mixer;

            //_output = AnimationScriptPlayable.Create(_graph, new RootMotionJob(), 1);
            //_output.ConnectInput(0, _mixer, 0);
        }

        private Playable GetOrCreatePlayable(UAnimation animation)
        {
            if (!_playables.TryGetValue(animation, out Playable animationPlayable, out int mixerIndex))
            {
                animationPlayable = animation.playable.Create(_graph);
                _playables[animation] = animationPlayable;
                _graph.Connect(animationPlayable, 0, _mixer, mixerIndex = _playables.Count - 1);

                int mixerInputCount = _mixer.GetInputCount();
                if (_playables.Count > mixerInputCount) _mixer.SetInputCount(mixerInputCount * 2);
            }
            return animationPlayable;
        }


        private void StartCurrentAnimation()
        {
            var animation = CurrentAnimation;
            var playInfo = PlayInfo;

            var animationPlayable = GetOrCreatePlayable(animation);

            NormalizedTime = 0;

            animationPlayable.SetSpeed(animation.speed * playInfo.speed);
        }

        public void Play(UAnimation animation, AnimationPlayInfo playInfo)
        {
            if (!playInfo.forcePlay && AnimationUtility.IsEqual(CurrentAnimation, animation)) return;
            ClearQueue();
            Queue(animation, playInfo);
        }

        public void Queue(UAnimation animation, AnimationPlayInfo playInfo)
        {
            _queue.Add(new QueuedAnimation
            {
                animation = animation,
                info = playInfo
            });
            if (_queue.Count == 1)
            {
                StartCurrentAnimation();
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        public void Stop()
        {
            if (!IsPlaying) return;

            _mixer.SetSpeed(0);
            _queue.Clear();
        }

        public void Update(float deltaTime)
        {
            if (CurrentAnimation.IsNull()) return;

            if (_queue.HasNext())
            {
                var info = _queue.PeekNext().info;
                if (CurrentAnimation.duration - Time <= info.blendTime)
                {
                    _queue.Next();
                    StartCurrentAnimation();
                    return;
                }
            }

            if (Time >= CurrentAnimation.duration)
            {
                if (CurrentAnimation.loop) StartCurrentAnimation();
                return;
            }

            int mixerIndex = _playables.IndexOf(CurrentAnimation);
            int count = _playables.Count;

            float t = 1;
            if (PlayInfo.blendTime > 0) t = Time / PlayInfo.blendTime;

            t = Mathf.Clamp01(t);

            float weightSum = 0;


            for (int i = count - 1; i >= 0; i--)
            {
                float weight = _mixer.GetInputWeight(i);
                float targetWeight = (i == mixerIndex) ? 1 : 0;

                if (Mathf.Approximately(weight, targetWeight)) continue;

                float newWeight = t;
                if (i != mixerIndex)
                {
                    newWeight = 1 - t;
                }

                _mixer.SetInputWeight(i, newWeight);

                weightSum += newWeight;
            }

            if (Mathf.Approximately(weightSum, 0))
            {
                _mixer.SetInputWeight(mixerIndex, 1);
            }

            _elapsedTime += deltaTime;
        }

    }
}