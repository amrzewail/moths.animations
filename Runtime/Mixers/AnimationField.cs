using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Attributes;
using Moths.Collections;
using Moths.Fields;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Moths.Animations
{

    public class AnimationField : GenericField<UAnimation> 
    {
        protected virtual void Reset()
        {
            value.speed = 1;
        }
    }


    [System.Serializable]
    public class AnimationReference : GenericReference<UAnimation, AnimationField, AnimationMB>, IAnimation
    {
        public AnimLayer layer => Value.layer;
        public string animID => Value.animID;
        public float speed => Value.speed;
        public bool loop => Value.loop;
        public bool applyIK => Value.applyIK;
        public Unique uniqueId => Value.uniqueId;

        public AnimationClip clip => Value.clip;

        public IPlayableCreator playable { get => Value.playable; }

        public bool IsValid()
        {
            return Value.IsValid();
        }

        public static implicit operator UAnimation(AnimationReference animation)
        {
            return UAnimation.ConstructFrom(animation);
        }
    }

}