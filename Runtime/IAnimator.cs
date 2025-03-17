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
        event Action<IAnimationState> AnimationPlayed;

        IAnimationState DefaultAnimation { get; }

        void Play(IAnimationState state);

        void Play(IAnimationState state, AnimationPlayInfo info);

        void Queue(IAnimationState state);
        void Queue(IAnimationState state, AnimationPlayInfo info);

        void Stop(int layer);

        void ClearQueue(int layer);

        bool IsPlaying(IAnimationState state);

        bool IsAnimationFinished(int layer);

        float GetNormalizedTime(int layer);

        void SetNormalizedTime(int layer, float time);

        IAnimationState GetCurrentAnimation(int layer);

        void SetRootMotion(bool value);
        void ResetRootMotion(Transform transform);

        void Pause(float pauseTime = 0);

        void Unpause();

        void SetSpeed(float speed);

        void SetFloat(string parameter, float value);

        float GetFloat(string parameter);

        bool[] GetCurrentPlayingLayers();

        Transform GetBoneTransform(HumanBodyBones bone);
    }
}