using UnityEngine;
using UnityEngine.Animations;

namespace Moths.Animations.Playables
{
    public struct RootMotionJob : IAnimationJob
    {
        public void ProcessAnimation(AnimationStream stream)
        {
            if (!stream.isHumanStream) return;
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            if (!stream.isHumanStream) return;
            Debug.Log(stream.rootMotionPosition + " " + stream.rootMotionRotation.eulerAngles);
        }
    }
}