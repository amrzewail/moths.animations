using UnityEngine;

namespace Moths.Animations.Utility
{
    public static class AnimationUtility
    {
        public static bool IsEqual(this IAnimation state, IAnimation other)
        {
            return state.uniqueId == other.uniqueId && state.clip == other.clip && state.layer == other.layer;
        }
    }
}