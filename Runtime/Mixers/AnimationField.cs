using Moths.Animations.Collections;
using Moths.Animations.Playables;
using Moths.Attributes;
using Moths.Fields;
using UnityEngine;

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
        public IPlayableCreator playable => Value.playable;
        public AnimLayer layer => Value.layer;
        public string animID => Value.animID;
        public AnimationClip clip => Value.clip;
        public float speed => Value.speed;

        public static implicit operator UAnimation(AnimationReference animation)
        {
            return UAnimation.ConstructFrom(animation);
        }
    }

}