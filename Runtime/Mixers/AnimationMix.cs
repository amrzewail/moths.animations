using Memory;
using Moths.Animations.Playables;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation Mix", menuName = "Moths/Animations/Animation Mix")]
    public class AnimationMix : AnimationField
    {
        [SerializeField] AnimationReference[] _animations;

        public override UAnimation GetValue()
        {
            value.playable = new MixAnimationCreator<AnimationReference>(_animations);
            return value;
        }
    }
}