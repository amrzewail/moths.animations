using Moths.Animations.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations
{
    [RequireComponent(typeof(Animator))]
    public partial class AnimatorPlayer : MonoBehaviour, IAnimator
    {
        [SerializeField] AnimationState _defaultAnimation;

        public IAnimationState DefaultAnimation => _defaultAnimation;

        public event Action<IAnimationState, AnimationPlayInfo> AnimationPlayed;

        private Animator _animator;
        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;

        private Layer _baseLayer;
        private IndexedDictionary<AvatarMask, Layer> _layers = new IndexedDictionary<AvatarMask, Layer>(16);
        private AnimationLayerMixerPlayable _layerMixer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _graph = PlayableGraph.Create(name);
            _output = AnimationPlayableOutput.Create(_graph, name, _animator);
            _layerMixer = AnimationLayerMixerPlayable.Create(_graph, _layers.Capacity + 1);

            for (int i = 0; i < _layers.Count + 1; i++)
            {
                _layerMixer.SetLayerAdditive((uint)i, false);
            }

            _baseLayer = new Layer(_graph);

            _layerMixer.SetInputWeight(0, 1);
            _graph.Connect(_baseLayer.Mixer, 0, _layerMixer, 0);
        }

        private void OnDestroy()
        {
            _graph.Destroy();
        }

        private void Start()
        {
            Play(DefaultAnimation);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _baseLayer.Update(deltaTime);
            for (int i = 0; i < _layers.Count; i++)
            {
                var layer = _layers[i];

                if (layer.IsPlaying)
                {
                    layer.Update(deltaTime);
                    _layers[i] = layer;

                    float weight = Mathf.MoveTowards(_layerMixer.GetInputWeight(i + 1), 1, deltaTime / Mathf.Max(layer.PlayInfo.blendTime, 0.00001f));
                    _layerMixer.SetInputWeight(i + 1, weight);
                }
                else
                {
                    _layerMixer.SetInputWeight(i + 1, Mathf.MoveTowards(_layerMixer.GetInputWeight(i + 1), 0, deltaTime / AnimationPlayInfo.BLEND_TIME));
                }
            }
        }

        private Layer GetLayer(int index)
        {
            if (index == 0)
            {
                return _baseLayer;
            }
            else
            {
                index--;
                if (index >= _layers.Count) return default;
                return _layers[index];
            }
        }

        private void SetLayer(int index, Layer layer)
        {
            if (index == 0)
            {
                _baseLayer = layer;
            }
            else
            {
                index--;
                if (index >= _layers.Count) return;
                _layers[index] = layer;
            }
        }

        public void ClearQueue(int layer)
        {
            var l = GetLayer(layer);
            l.ClearQueue();
            SetLayer(layer, l);
        }

        public IAnimationState GetCurrentAnimation(int layer)
        {
            return GetLayer(layer).CurrentState;
        }

        public bool[] GetCurrentPlayingLayers()
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string parameter)
        {
            throw new NotImplementedException();
        }

        public float GetNormalizedTime(int layer)
        {
            var l = GetLayer(layer);
            return l.NormalizedTime;
        }

        public bool IsAnimationFinished(int layer)
        {
            return GetLayer(layer).IsFinished;
        }

        public bool IsPlaying(IAnimationState state)
        {
            return GetLayer(state.layer).CurrentState == state;
        }

        public void Pause(float pauseTime = 0)
        {
            throw new NotImplementedException();
        }

        public void Play(IAnimationState state)
        {
            Play(state, AnimationPlayInfo.Default);
        }

        public void Play(IAnimationState state, AnimationPlayInfo info)
        {
            ClearQueue(state.layer);
            Queue(state, info);
        }

        public void Queue(IAnimationState state)
        {
            Queue(state, AnimationPlayInfo.Default);
        }

        public void Queue(IAnimationState state, AnimationPlayInfo info)
        {
            Layer layer = _baseLayer;
            int mixerIndex = 0;
            if (state.mask)
            {
                if (!_layers.Contains(state.mask))
                {
                    layer = _layers[state.mask] = new Layer(_graph);
                    mixerIndex = _layers.IndexOf(state.mask) + 1;

                    _layerMixer.SetLayerMaskFromAvatarMask((uint)mixerIndex, state.mask);
                    _graph.Connect(layer.Mixer, 0, _layerMixer, mixerIndex);
                }
                else
                {
                    layer = _layers[state.mask];
                    mixerIndex = _layers.IndexOf(state.mask) + 1;
                }
            }

            layer.Queue(state, info);

            if (mixerIndex == 0)
            {
                _baseLayer = layer;
            }
            else
            {
                _layers[state.mask] = layer;
            }

            _output.SetSourcePlayable(_layerMixer);

            _graph.Play();
        }

        public void SetFloat(string parameter, float value)
        {
            throw new NotImplementedException();
        }

        public void SetNormalizedTime(int layer, float time)
        {
            throw new NotImplementedException();
        }

        public void SetRootMotion(bool value)
        {
            throw new NotImplementedException();
        }

        public void SetSpeed(float speed)
        {
            throw new NotImplementedException();
        }

        public void Stop(int layer)
        {
            var l = GetLayer(layer);
            l.Stop();
            SetLayer(layer, l);
        }

        public void Unpause()
        {
            throw new NotImplementedException();
        }
    }
}