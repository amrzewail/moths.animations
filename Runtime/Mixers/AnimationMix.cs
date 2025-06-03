using Memory;
using Moths.Animations.Playables;
using Moths.Collections;
using Moths.Fields;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation Mix", menuName = "Moths/Animations/Animation Mix")]

    [HideValue]
    public class AnimationMix : AnimationField
    {
        [SerializeField] Unique _uniqueId;
        [SerializeField] StringReference _animID;
        [SerializeField] AvatarMask _mask;
        [SerializeField] AnimationReference[] _animations;

        public override UAnimation GetValue()
        {
            value.uniqueId = _uniqueId;
            value.animID = _animID;
            value.mask = _mask;
            value.playable = new MixAnimationCreator<AnimationReference>(_animations);
            value.speed = 1;
            for (int i = 0; i < _animations.Length; i++) value.speed *= _animations[i].speed;
            return value;
        }
    }
}