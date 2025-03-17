using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anima
{
    public partial class AnimatorPlayer
    {
        [SerializeField] Transform _rootMotionTarget;

        private Vector3 _deltaPosition;
        private Quaternion _deltaRotation;

        void OnAnimatorMove()
        {
            Quaternion deltaRotation = _animator.deltaRotation;
            Vector3 deltaPosition = _animator.deltaPosition;

            _deltaPosition += deltaPosition;
            _deltaRotation *= deltaRotation;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _rootMotionTarget.transform.position += _deltaPosition;
            _rootMotionTarget.transform.rotation *= _deltaRotation;

            _deltaPosition = Vector3.zero;
            _deltaRotation = Quaternion.identity;
        }
    }
}