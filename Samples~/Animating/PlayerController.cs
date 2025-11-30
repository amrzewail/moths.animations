using Moths.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed;

    [SerializeField] AnimationReference _idle;
    [SerializeField] AnimationReference _walk;
    [SerializeField] AnimationReference _jump;
    [SerializeField] AnimationReference _attack;

    private IAnimator _animator;
    private State _state = State.Idle;

    private enum State
    {
        Idle,
        Walk,
        Jump,
    }

    private void Start()
    {
        _animator = GetComponent<IAnimator>();
    }

    void Update()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey("a")) horizontal = -1;
        if (Input.GetKey("d")) horizontal = 1;

        if (Input.GetKey("w")) vertical = 1;
        if (Input.GetKey("s")) vertical = -1;

        bool isMoving = !Mathf.Approximately(horizontal * 10 + vertical, 0);
        bool isJump = Input.GetKeyDown("f");

        switch (_state)
        {
            case State.Idle:
                _animator.Play(_idle);
                if (isMoving) _state = State.Walk;
                if (isJump) _state = State.Jump;
                break;

            case State.Walk:
                if (isMoving)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(horizontal, 0, vertical), Vector3.up), 360 * Time.deltaTime);
                    transform.position += Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical), 1) * _moveSpeed * Time.deltaTime;
                    _animator.Play(_walk);
                }
                else
                {
                    _animator.Stop(_walk.Value.layer);
                    _state = State.Idle;
                }
                if (isJump) _state = State.Jump;
                break;

            case State.Jump:
                _animator.Play(_jump);
                if (_animator.IsAnimationFinished(_jump.layer))
                {
                    _state = State.Idle;
                }
                break;

        }
    }
}
