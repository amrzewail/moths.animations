using UnityEngine;

namespace Moths.Animations
{
    public static class AnimationStateUtility
    {
        public static bool IsEqual(this IAnimationState state, IAnimationState other)
        {
            return state.stateName == other.stateName && state.layer == other.layer && state.duration == other.duration;
        }
    }
}