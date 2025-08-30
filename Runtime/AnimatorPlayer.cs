using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moths.Animations
{
    public partial class AnimatorPlayer : MonoBehaviour, IAnimator
    {
        private struct AnimationQueue
        {
            public IAnimationState state;
            public AnimationPlayInfo info;
        }

        private Animator _animator;
        private AnimatorLayer[] _layers;
        private bool[] _usedLayers;
        private IAnimationState[] _statesArr = new IAnimationState[1];
        private Dictionary<int, List<AnimationQueue>> _queue = new Dictionary<int, List<AnimationQueue>>();
        private bool[] _currentPlayingQueue = null;

        private float _animatorSpeed = 1;
        private float _isPausedSpeed = 1;


        public event Action<IAnimationState, AnimationPlayInfo> AnimationPlayed;

        [SerializeField] AnimationState _defaultAnimation;

        public string[] noBlendTimeAnimations;
        public AnimatorLayer[] layers => _layers;

        public IAnimationState DefaultAnimation => _defaultAnimation;
        public Constraint PositionConstraints { get => _lockPosition; set => _lockPosition = value; }

        private void Awake()
        {
            if (!_animator) _animator = GetComponent<Animator>();
            _layers = new AnimatorLayer[_animator.layerCount];
            for (int i = 0; i < _layers.Length; i++) _layers[i] = new AnimatorLayer();

            if (_usedLayers == null) _usedLayers = new bool[layers.Length];
            if (_currentPlayingQueue == null) _currentPlayingQueue = new bool[layers.Length];

        }

        private void Start()
        {
            if (DefaultAnimation != null)
            {
                Play(DefaultAnimation);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i].Update(_animator);

                if (_usedLayers[i] == false || (_layers[i].playInfo.preserve && IsAnimationFinished(i)))
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

        private void PlayInternal(IAnimationState state, AnimationPlayInfo info, bool clearQueue)
        {
            if (state.animID == "__StopID__")
            {
                Stop(state.layer);
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


            if (state.combine == null)
            {
                if (state.layer >= _layers.Length) return;

                var animLayer = _layers[state.layer];
                if (animLayer.Play(_animator, state.layer, noBlendTimeAnimations, state, info))
                {
                    AnimationPlayed?.Invoke(state, info);
                }

                return;
            }

            IAnimationState[] states = state.combine;

            for (int i = 0; i < states.Length; i++)
            {
                PlayInternal(states[i], info, clearQueue);
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

        private void AppendUsedLayers(IAnimationState state, bool[] layers, int iteration = 0)
        {
            int layerIndex = state.layer;
            if (layerIndex >= layers.Length) return;
            if (state.combine != null)
            {
                foreach (var s in state.combine)
                {
                    AppendUsedLayers(s, layers, ++iteration);
                }
            }
            layers[layerIndex] = true;
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

        public void PlayNoClearQueue(IAnimationState state, AnimationPlayInfo info)
        {
            if (!info.appendToLayers)
            {
                ResetUsedLayers();
            }
            AppendUsedLayers(state, _usedLayers);

            PlayInternal(state, info, false);
        }

        public void Play(IAnimationState state)
        {
            Play(state, AnimationPlayInfo.Default);
        }

        public void Play(IAnimationState state, AnimationPlayInfo info)
        {
            if (!info.appendToLayers)
            {
                ResetUsedLayers();
            }
            AppendUsedLayers(state, _usedLayers);

            PlayInternal(state, info, true);
        }

        public void Stop(int layer)
        {
            _layers[layer].Stop();
            _usedLayers[layer] = false;
            _animator.Play("Empty", layer);
        }

        public void Queue(IAnimationState state)
        {
            Queue(state, AnimationPlayInfo.Default);
        }

        public void Queue(IAnimationState state, AnimationPlayInfo info)
        {
            if (!_queue.ContainsKey(state.layer))
            {
                _queue.Add(state.layer, new List<AnimationQueue>());
            }
            _queue[state.layer].Add(new AnimationQueue
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

        public bool IsPlaying(IAnimationState state)
        {
            if (state.layer >= _layers.Length) return false;

            return _layers[state.layer].IsPlaying(state);
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
            if (_layers[layer].currentAnimation == null) return;
            if (Mathf.Approximately(_animator.speed, 0)) return;
            _layers[layer].normalizedTime = time;
        }

        public int GetCurrentFrame(int layer)
        {
            if (layer >= _layers.Length) return 0;

            return _layers[layer].currentFrame;
        }

        public IAnimationState GetCurrentAnimation(int layer)
        {
            if (_layers == null)
            {
                Awake();
            }

            if (layer >= _layers.Length) return null;

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

        public class AnimatorLayer
        {
            private AnimatorStateInfo _stateInfo;
            private AnimationPlayInfo _playInfo = AnimationPlayInfo.Default;
            private float _currentSpeed;
            private bool _currentMirror;
            private bool _changedAnimation;

            public float normalizedTime;
            public IAnimationState currentAnimation;
            public bool isAnimationFinished;
            public int currentFrame;

            public AnimationPlayInfo playInfo => _playInfo;

            public void Update(Animator animator)
            {
                float normalizedExitTime = 1.0f;
                if (currentAnimation != null) normalizedExitTime = (currentAnimation.duration - _playInfo.exitRangeSecs) / currentAnimation.duration;
                if (_changedAnimation && normalizedTime < normalizedExitTime)
                {
                    _changedAnimation = false;
                }
                if (!_changedAnimation) isAnimationFinished = normalizedTime >= normalizedExitTime;

                currentFrame = Mathf.FloorToInt(normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * 30));
            }

            public bool Play(Animator animator, int layer, string[] noBlendTimeAnimations, IAnimationState state, AnimationPlayInfo info)
            {
                if (!IsPlaying(state) || isAnimationFinished || info.forcePlay)
                {

                    Debug.Log("Play animation: " + state.stateName + " forcePlay: " + info.forcePlay + " finished: " + isAnimationFinished);

                    _playInfo = info;

                    animator.CrossFadeInFixedTime(state.stateName, noBlendTimeAnimations.Contains(state.stateName) ? 0 : info.blendTime, layer, state.duration * info.normalizedTime);
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
                currentAnimation = null;
            }

            public bool IsPlaying(IAnimationState state)
            {
                if (currentAnimation == null) return false;
                return currentAnimation.IsEqual(state);
            }

        }
    }
}