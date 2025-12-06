using UnityEngine;

namespace Moths.Animations
{
    [RequireComponent(typeof(IAnimator))]
    public class RigidbodyRootMotion : MonoBehaviour
    {
        private IAnimator _animator;
        private RootMotion _rootMotion;

        private Vector3 _deltaPosition;
        private Quaternion _deltaRotation;

        [SerializeField] Rigidbody _rigidbody;

        public Vector3 PositionMultiplier { get; set; } = Vector3.one;

        private void Awake()
        {
            _animator = GetComponent<IAnimator>();

            _rootMotion = new RootMotion(Vector3.zero, Quaternion.identity);

            _deltaPosition = Vector3.zero;
            _deltaRotation = Quaternion.identity;
        }

        private void LateUpdate()
        {
            _rootMotion = _animator.RootMotion.Damp(_rootMotion);

            _deltaPosition += Vector3.Scale(_rootMotion.DeltaPosition, PositionMultiplier);
            _deltaRotation = _deltaRotation * _rootMotion.DeltaRotation;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(_rigidbody.position + _deltaPosition);
            _rigidbody.MoveRotation(_rigidbody.rotation * _deltaRotation);

            _deltaPosition = Vector3.zero;
            _deltaRotation = Quaternion.identity;
        }
    }
}