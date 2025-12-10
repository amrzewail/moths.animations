using UnityEngine;

namespace Moths.Animations
{
    [RequireComponent(typeof(IAnimator))]
    public class TransformRootMotion : MonoBehaviour, IRootMotionApplier
    {
        private IAnimator _animator;
        private RootMotion _rootMotion;

        private Vector3 _deltaPosition;
        private Quaternion _deltaRotation;

        [SerializeField] Transform _transform;

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

            _transform.position = _transform.position + _deltaPosition;
            _transform.rotation = _transform.rotation * _deltaRotation;

            _deltaPosition = Vector3.zero;
            _deltaRotation = Quaternion.identity;
        }
    }
}