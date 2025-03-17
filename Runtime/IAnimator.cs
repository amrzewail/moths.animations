using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anima
{
    [System.Serializable]
    public struct AnimationPlayInfo
    {
        public const float BLEND_TIME = 0.2f;

        public float speed;
        public bool mirror;
        public float blendTime;
        public float normalizedTime;
        public bool appendToLayers;
        public bool preserve;
        public bool forcePlay;
        public float exitRangeSecs;

        public static AnimationPlayInfo Default = new AnimationPlayInfo(blendTime: BLEND_TIME);

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
        public event Action<IAnimationState> AnimationPlayed;

        public IAnimationState DefaultAnimation { get; }

        public void Play(IAnimationState state);

        public void Play(IAnimationState state, AnimationPlayInfo info);

        public void Queue(IAnimationState state);
        public void Queue(IAnimationState state, AnimationPlayInfo info);

        public void Stop(int layer);

        public void ClearQueue(int layer);

        public bool IsPlaying(IAnimationState state);

        public bool IsAnimationFinished(int layer);

        public float GetNormalizedTime(int layer);

        public void SetNormalizedTime(int layer, float time);

        public IAnimationState GetCurrentAnimation(int layer);

        public void SetRootMotion(bool value);

        public void Pause(float pauseTime = 0);

        public void Unpause();

        public void SetSpeed(float speed);

        //public void ResetRootMotion(IActor actor);

        public void SetFloat(string parameter, float value);

        public float GetFloat(string parameter);

        public bool[] GetCurrentPlayingLayers();
    }
}