using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public partial class AnimatorPlayer
    {
        [System.Flags]
        public enum Constraint
        {
            None = 0, X = 1 << 0, Y = 1 << 1, Z = 1 << 2
        };

        [Header("Root Motion")]
        [SerializeField] bool _applyRootMotion = true;
        [SerializeField] Transform _rootMotionTarget;
        [SerializeField] Constraint _lockPosition;

        private Vector3 _deltaPosition = Vector3.zero;
        private Quaternion _deltaRotation = Quaternion.identity;

        void OnAnimatorMove()
        {
            if (!_rootMotionTarget) return;
            if (!ApplyRootMotion) return;

            Quaternion deltaRotation = _animator.deltaRotation;
            Vector3 deltaPosition = _animator.deltaPosition;

            _deltaPosition += deltaPosition;
            _deltaRotation *= deltaRotation;

            //transform.position -= deltaPosition;
            //transform.rotation *= Quaternion.Inverse(deltaRotation);

            if ((_lockPosition & Constraint.X) != 0)
            {
                _deltaPosition -= transform.right * Vector3.Dot(transform.right, _deltaPosition);
            }

            if ((_lockPosition & Constraint.Y) != 0)
            {
                _deltaPosition -= transform.up * Vector3.Dot(transform.up, _deltaPosition);
            }

            if ((_lockPosition & Constraint.Z) != 0)
            {
                _deltaPosition -= transform.forward * Vector3.Dot(transform.forward, _deltaPosition);
            }

            _rootMotionTarget.position += _deltaPosition;
            _rootMotionTarget.rotation *= _deltaRotation;

            _deltaPosition = Vector3.zero;
            _deltaRotation = Quaternion.identity;
        }
    }
}