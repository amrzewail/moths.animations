using Moths.Animations.Collections;
using Moths.Animations.Utility;
using Moths.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations
{
    public partial class AnimatorPlayer : MonoBehaviour, IAnimator
    {
        [SerializeField] Animator _animator;
        [SerializeField] AnimationReference _defaultAnimation;

        public UAnimation DefaultAnimation => _defaultAnimation.Value;
        public bool ApplyRootMotion { get => _applyRootMotion; set => _applyRootMotion = value; }

        public event Action<UAnimation, AnimationPlayInfo> AnimationPlayed;

        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;

        private IndexedDictionary<AnimLayer, LayerMixer> _layerPlayers = new IndexedDictionary<AnimLayer, LayerMixer>(16);
        private AnimationLayerMixerPlayable _layerMixer;

        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            _graph = PlayableGraph.Create(name);
            _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            _output = AnimationPlayableOutput.Create(_graph, name, _animator);

            _layerMixer = AnimationLayerMixerPlayable.Create(_graph, _layerPlayers.Capacity + 1);

            for (int i = 0; i < _layerPlayers.Count + 1; i++)
            {
                _layerMixer.SetLayerAdditive((uint)i, false);
            }

            _layerPlayers[new AnimLayer(null)] = new LayerMixer(_graph);

            _layerMixer.SetInputWeight(0, 1);

            _output.SetSourcePlayable(_layerMixer);

            _graph.Connect(_layerPlayers[0].Mixer, 0, _layerMixer, 0);

            _graph.Play();
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
            float timeScale = Time.timeScale;

            for (int i = 0; i < _layerPlayers.Count; i++)
            {
                var layer = _layerPlayers[i];

                layer.Update(deltaTime);
                _layerPlayers[i] = layer;

                if (layer.IsPlaying)
                {
                    _layerMixer.SetInputWeight(i, Mathf.MoveTowards(_layerMixer.GetInputWeight(i), 1, deltaTime / Mathf.Max(layer.PlayInfo.blendTime, 0.00001f)));
                }
                else if (i > 0)
                {
                    _layerMixer.SetInputWeight(i, Mathf.MoveTowards(_layerMixer.GetInputWeight(i), 0, deltaTime / AnimationPlayInfo.BLEND_TIME));
                }
            }

            _graph.Evaluate(deltaTime);
        }

        private void CreateLayer(AnimLayer layer)
        {
            if (_layerPlayers.Contains(layer)) return;
            var l = _layerPlayers[layer] = new LayerMixer(_graph);
            int mixerIndex = _layerPlayers.IndexOf(layer);

            _layerMixer.SetLayerMaskFromAvatarMask((uint)mixerIndex, layer.Mask);
            _graph.Connect(l.Mixer, 0, _layerMixer, mixerIndex);
        }

        public void ClearQueue(AnimLayer layer)
        {
            if (_layerPlayers.TryGetValue(layer, out var value, out int index))
            {
                value.ClearQueue();
                _layerPlayers[index] = value;
            }
        }

        public IAnimation GetCurrentAnimation(AnimLayer layer)
        {
            return _layerPlayers[layer].CurrentAnimation;
        }

        public bool[] GetCurrentPlayingLayers()
        {
            throw new NotImplementedException();
        }

        public float GetNormalizedTime(AnimLayer layer)
        {
            return _layerPlayers[layer].NormalizedTime;
        }

        public void SetNormalizedTime(AnimLayer layer, float time)
        {
            var player = _layerPlayers[layer];
            player.NormalizedTime = time;
            _layerPlayers[layer] = player;
        }

        public bool IsAnimationFinished(AnimLayer layer)
        {
            return _layerPlayers[layer].IsFinished;
        }

        public bool IsPlaying<TAnimation>(TAnimation animation) where TAnimation : IAnimation
        {
            return AnimationUtility.IsEqual(_layerPlayers[animation.layer].CurrentAnimation, animation);
        }

        public void Play<TAnimation>(TAnimation animation) where TAnimation : IAnimation
        {
            Play(animation, AnimationPlayInfo.Default);
        }

        public void Play<TAnimation>(TAnimation animation, AnimationPlayInfo info) where TAnimation : IAnimation
        {
            CreateLayer(animation.layer);
            var layer = _layerPlayers[animation.layer];
            layer.Play(UAnimation.ConstructFrom(animation), info);
            _layerPlayers[animation.layer] = layer;
        }

        public void Queue<TAnimation>(TAnimation animation) where TAnimation : IAnimation
        {
            Queue(animation, AnimationPlayInfo.Default);
        }

        public void Queue<TAnimation>(TAnimation animation, AnimationPlayInfo info) where TAnimation : IAnimation
        {
            CreateLayer(animation.layer);
            var layer = _layerPlayers[animation.layer];
            layer.Queue(UAnimation.ConstructFrom(animation), info);
            _layerPlayers[animation.layer] = layer;
        }


        public void SetSpeed(float speed)
        {
            _layerMixer.SetSpeed(speed);
        }

        public void Stop(AnimLayer layer)
        {
            if (_layerPlayers.TryGetValue(layer, out var value, out int index))
            {
                value.Stop();
                _layerPlayers[index] = value;
            }
        }

        public void Pause(float pauseTime = 0)
        {
            _layerMixer.Pause();
        }

        public void Unpause()
        {
            _layerMixer.Play();
        }

        public void SetFloat(string parameter, float value)
        {
            throw new NotImplementedException();
        }
        public float GetFloat(string parameter)
        {
            throw new NotImplementedException();
        }

    }
}