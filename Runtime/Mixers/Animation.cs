using Moths.Animations.Playables;
using UnityEngine;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation", menuName = "Moths/Animations/Animation")]
    public class Animation : AnimationField
    {
        public override UAnimation Value
        {
            get
            {
                value.playable = new BasicAnimationCreator<UAnimation>(value);
                return value;
            }
            set => base.Value = value;
        }
    }
}