using UnityEngine;

namespace Moths.Animations
{
    public interface IRootMotionApplier
    {
        public Vector3 PositionMultiplier { get; set; }
    }
}