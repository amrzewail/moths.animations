using Moths.Animations.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    [System.Serializable]
    public struct AnimationPlayInfo
    {
        public const float BLEND_TIME = 0.4f;

        public float speed;
        public bool mirror;
        public float blendTime;
        public float normalizedTime;
        public bool appendToLayers;
        public bool preserve;
        public bool forcePlay;
        public float exitRangeSecs;

        public static readonly AnimationPlayInfo Default = new AnimationPlayInfo(blendTime: BLEND_TIME);

        public AnimationPlayInfo(int _ = 0)
        {
            this = Default;
        }

        public AnimationPlayInfo(
            float speed = 1,
            bool mirror = false,
            float blendTime = BLEND_TIME,
            float normalizedTime = 0,
            bool appendToLayers = false,
            bool preserve = false,
            bool forcePlay = false,
            float exitRangeSecs = 0)
        {
            this.speed = speed;
            this.mirror = mirror;
            this.blendTime = blendTime;
            this.normalizedTime = normalizedTime;
            this.appendToLayers = appendToLayers;
            this.preserve = preserve;
            this.forcePlay = forcePlay;
            this.exitRangeSecs = exitRangeSecs;
        }
    }

    public interface IAnimator
    {
        public event Action<UAnimation, AnimationPlayInfo> AnimationPlayed;
        public UAnimation DefaultAnimation { get; }

        public bool ApplyRootMotion { get; set; }

        public void Play<TAnimation>(TAnimation state) where TAnimation : IAnimation;

        public void Play<TAnimation>(TAnimation state, AnimationPlayInfo info) where TAnimation : IAnimation;

        public void Queue<TAnimation>(TAnimation state) where TAnimation : IAnimation;
        public void Queue<TAnimation>(TAnimation state, AnimationPlayInfo info) where TAnimation : IAnimation;

        public void Stop(AnimLayer layer);

        public void ClearQueue(AnimLayer layer);

        public bool IsPlaying<TAnimation>(TAnimation state) where TAnimation : IAnimation;

        public bool IsAnimationFinished(AnimLayer layer);

        public float GetNormalizedTime(AnimLayer layer);

        public IAnimation GetCurrentAnimation(AnimLayer layer);

        public void Pause(float pauseTime = 0);

        public void Unpause();

        public void SetSpeed(float speed);

        public void SetFloat(string parameter, float value);

        public float GetFloat(string parameter);
    }
}