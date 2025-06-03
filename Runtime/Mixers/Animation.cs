using Moths.Animations.Playables;
using UnityEngine;

namespace Moths.Animations
{
    [CreateAssetMenu(fileName = "Animation", menuName = "Moths/Animations/Animation")]
    public class Animation : AnimationField
    {
        public override UAnimation GetValue()
        {
            value.playable = new BasicAnimationCreator<UAnimation>(value);
            return value;
        }
    }
}