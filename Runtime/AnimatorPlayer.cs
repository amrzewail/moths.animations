using Moths.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public partial class AnimatorPlayer : MonoBehaviour, IAnimator, IAnimator.INormalizedTimeSetter
    {
        private struct AnimationQueue
        {
            public AnimatorState state;
            public AnimationPlayInfo info;
        }

        [System.Flags]
        public enum Constraint
        {
            None = 0, X = 1 << 0, Y = 1 << 1, Z = 1 << 2
        };

        public event Action<AnimatorState, AnimationPlayInfo> AnimationPlayed;

        public Animator Animator => _animator;
        public AnimatorLayer[] layers => _layers;
        public RootMotion RootMotion { get; private set; }
        public AnimatorState DefaultAnimation => _defaultAnimation;
        public Constraint PositionConstraints { get => _lockPosition; set => _lockPosition = value; }

        [SerializeField] OptionalProperty<AnimatorState> _defaultAnimation;
        [SerializeField] Constraint _lockPosition;

        private Animator _animator;
        private AnimatorLayer[] _layers;
        private bool[] _usedLayers;
        private Dictionary<int, List<AnimationQueue>> _queue = new Dictionary<int, List<AnimationQueue>>();
        private bool[] _currentPlayingQueue = null;

        private Vector3 _deltaPosition = Vector3.zero;
        private Quaternion _deltaRotation = Quaternion.identity;

        private float _animatorSpeed = 1;
        private float _isPausedSpeed = 1;


        private void Awake()
        {
            if (!_animator) _animator = GetComponent<Animator>();
            _layers = new AnimatorLayer[_animator.layerCount];
            for (int i = 0; i < _layers.Length; i++) _layers[i] = new AnimatorLayer();

            _usedLayers = new bool[layers.Length];
            _currentPlayingQueue = new bool[layers.Length];
        }

        private void Start()
        {
            if (_defaultAnimation)
            {
                Play(DefaultAnimation);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i].Update(_animator);

                if (_usedLayers[i] == false || (i > 0 && IsAnimationFinished(i)))
                {
                    _layers[i].Stop();
                    _usedLayers[i] = false;
                }

                if (i == 0) continue;

                float weightTarget = _usedLayers[i] ? 1 : 0;
                _animator.SetLayerWeight(i, Mathf.MoveTowards(_animator.GetLayerWeight(i), weightTarget, Time.deltaTime / AnimationPlayInfo.BLEND_TIME));
            }


            List<AnimationQueue> queue;
            int layer;
            foreach (var pair in _queue)
            {
                layer = pair.Key;
                queue = pair.Value;

                if (_currentPlayingQueue[layer] && IsAnimationFinished(layer))
                {
                    _currentPlayingQueue[layer] = false;
                }


                if (queue.Count == 0) continue;

                if (_currentPlayingQueue[layer] == false)
                {
                    if (!_layers[layer].IsPlaying(queue[0].state) || queue[0].info.forcePlay)
                    {
                        _currentPlayingQueue[layer] = true;
                        PlayNoClearQueue(queue[0].state, queue[0].info);
                        queue.RemoveAt(0);
                        continue;
                    }
                }
            }
        }
        void OnAnimatorMove()
        {
            Quaternion deltaRotation = _animator.deltaRotation;
            Vector3 deltaPosition = _animator.deltaPosition;

            _deltaPosition = deltaPosition;
            _deltaRotation = deltaRotation;

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

            RootMotion = new RootMotion(_deltaPosition, _deltaRotation);
        }

        private void PlayInternal(AnimatorState state, AnimationPlayInfo info, bool clearQueue)
        {
            if (state.StateName == "$__StopID__$")
            {
                Stop(state.Layer);
                return;
            }

            if (clearQueue)
            {
                for (int i = 0; i < _layers.Length; i++)
                {
                    ClearQueue(i);
                }
                clearQueue = false;
            }

            if (state.Layer >= _layers.Length) return;

            var animLayer = _layers[state.Layer];
            if (animLayer.Play(_animator, state.Layer, state, info))
            {
                AnimationPlayed?.Invoke(state, info);
            }
        }

        private void ResetUsedLayers()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i].playInfo.preserve) continue;
                _usedLayers[i] = false;
            }
        }

        public void ResetRootMotion(Transform transform)
        {
            //SetRootMotion(false);

            transform.transform.eulerAngles = _animator.transform.eulerAngles;
            transform.transform.position = _animator.transform.position;

            _animator.transform.localPosition = Vector3.zero;
            _animator.transform.localEulerAngles = Vector3.zero;

        }

        public void SetRootMotion(bool value)
        {
            _animator.applyRootMotion = value;
        }

        public void PlayNoClearQueue(AnimatorState state, AnimationPlayInfo info)
        {
            if (!info.appendToLayers)
            {
                ResetUsedLayers();
            }
            PlayInternal(state, info, false);
        }

        public void Play(AnimatorState state)
        {
            Play(state, AnimationPlayInfo.Default);
        }

        public void Play(AnimatorState state, AnimationPlayInfo info)
        {
            if (!info.appendToLayers)
            {
                ResetUsedLayers();
            }
            PlayInternal(state, info, true);
        }

        public void Stop(int layer)
        {
            _layers[layer].Stop();
            _usedLayers[layer] = false;
            _animator.Play("Empty", layer);
        }

        public void Queue(AnimatorState state)
        {
            Queue(state, AnimationPlayInfo.Default);
        }

        public void Queue(AnimatorState state, AnimationPlayInfo info)
        {
            if (!_queue.ContainsKey(state.Layer))
            {
                _queue.Add(state.Layer, new List<AnimationQueue>());
            }
            _queue[state.Layer].Add(new AnimationQueue
            {
                state = state,
                info = info
            });
        }

        public void ClearQueue(int layer)
        {
            if (!_queue.ContainsKey(layer)) return;
            _queue[layer].Clear();
            _currentPlayingQueue[layer] = false;
        }

        public bool IsPlaying(AnimatorState state)
        {
            if (state.Layer >= _layers.Length) return false;

            return _layers[state.Layer].IsPlaying(state);
        }

        public bool IsAnimationFinished(AnimatorState state)
        {
            return !IsPlaying(state) || IsAnimationFinished(state.Layer);
        }

        public bool IsAnimationFinished(int layer)
        {
            if (layer >= _layers.Length) return false;

            return _layers[layer].isAnimationFinished;
        }

        public float GetNormalizedTime(int layer)
        {
            if (layer >= _layers.Length) return 0;

            return _layers[layer].normalizedTime;
        }

        public void SetNormalizedTime(int layer, float time)
        {
            if (layer >= _layers.Length) return;
            if (!_layers[layer].currentAnimation.IsValid) return;
            if (Mathf.Approximately(_animator.speed, 0)) return;
            _layers[layer].normalizedTime = time;
        }

        public AnimatorState GetCurrentAnimation(int layer)
        {
            if (_layers == null)
            {
                Awake();
            }

            if (layer >= _layers.Length) return default;

            return _layers[layer].currentAnimation;
        }

        public bool[] GetCurrentPlayingLayers()
        {
            return _usedLayers;
        }

        public void Pause(float pauseTime = 0)
        {
            _animator.speed = _animatorSpeed * (_isPausedSpeed = 0);
            if (!Mathf.Approximately(pauseTime, 0))
            {
                Invoke(nameof(Unpause), pauseTime);
            }
        }

        public void Unpause()
        {
            _animator.speed = _animatorSpeed * (_isPausedSpeed = 1);
        }

        public void SetSpeed(float speed)
        {
            _animator.speed = (_animatorSpeed = speed) * _isPausedSpeed;
        }

        public void SetFloat(string parameter, float value)
        {
            _animator.SetFloat(parameter, value);
        }

        public float GetFloat(string parameter)
        {
            return _animator.GetFloat(parameter);
        }

        public Transform GetBoneTransform(HumanBodyBones bone)
        {
            return _animator.GetBoneTransform(bone);
        }

        public struct AnimatorLayer
        {
            private AnimationPlayInfo _playInfo;
            private float _currentSpeed;
            private bool _currentMirror;
            private bool _changedAnimation;

            public float normalizedTime;
            public AnimatorState currentAnimation;
            public bool isAnimationFinished;
            public int currentFrame;

            public AnimationPlayInfo playInfo => _playInfo;

            public void Update(Animator animator)
            {
                float normalizedExitTime = 1.0f;
                if (currentAnimation.IsValid) normalizedExitTime = (currentAnimation.Duration - _playInfo.exitRangeSecs) / currentAnimation.Duration;
                if (_changedAnimation && normalizedTime < normalizedExitTime)
                {
                    _changedAnimation = false;
                }
                if (!_changedAnimation) isAnimationFinished = normalizedTime >= normalizedExitTime;

                currentFrame = Mathf.FloorToInt(normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * 30));
            }

            public bool Play(Animator animator, int layer, AnimatorState state, AnimationPlayInfo info)
            {
                if (!IsPlaying(state) || isAnimationFinished || info.forcePlay)
                {

                    Debug.Log("Play animation: " + state.StateName + " forcePlay: " + info.forcePlay + " finished: " + isAnimationFinished);

                    _playInfo = info;
                    animator.CrossFadeInFixedTime(state.StateName, info.blendTime, layer, state.Duration * info.normalizedTime);
                    animator.SetFloat("Speed", info.speed);
                    _currentSpeed = info.speed;
                    animator.SetBool("Mirror", info.mirror);
                    _currentMirror = info.mirror;
                    currentAnimation = state;
                    normalizedTime = info.normalizedTime;
                    isAnimationFinished = false;
                    _changedAnimation = true;
                }
                else
                {
                    if (info.speed != _currentSpeed)
                    {
                        _playInfo.speed = info.speed;

                        animator.SetFloat("Speed", info.speed);
                        _currentSpeed = info.speed;
                    }
                    if (info.mirror != _currentMirror)
                    {
                        _playInfo.mirror = info.mirror;

                        animator.SetBool("Mirror", info.mirror);
                        _currentMirror = info.mirror;
                    }
                }
                return _changedAnimation;
            }

            public void Stop()
            {
                currentAnimation = default;
            }

            public bool IsPlaying(AnimatorState state)
            {
                if (!currentAnimation.IsValid) return false;
                return currentAnimation.Equals(state);
            }

        }
    }
}