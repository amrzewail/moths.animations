using Moths.Animations.Collections;
using Moths.Animations.Utility;
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
        [SerializeField] AnimationReference _defaultAnimation;

        public UAnimation DefaultAnimation => _defaultAnimation.Value;
        public bool ApplyRootMotion { get => _applyRootMotion; set => _applyRootMotion = value; }

        public event Action<UAnimation, AnimationPlayInfo> AnimationPlayed;

        private Animator _animator;
        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;

        private IndexedDictionary<AnimLayer, LayerPlayer> _layerPlayers = new IndexedDictionary<AnimLayer, LayerPlayer>(16);
        private AnimationLayerMixerPlayable _layerMixer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _graph = PlayableGraph.Create(name);
            _output = AnimationPlayableOutput.Create(_graph, name, _animator);
            _layerMixer = AnimationLayerMixerPlayable.Create(_graph, _layerPlayers.Capacity + 1);

            for (int i = 0; i < _layerPlayers.Count + 1; i++)
            {
                _layerMixer.SetLayerAdditive((uint)i, false);
            }

            _layerPlayers[new AnimLayer(null)] = new LayerPlayer(_graph);

            _layerMixer.SetInputWeight(0, 1);
            _graph.Connect(_layerPlayers[0].Mixer, 0, _layerMixer, 0);
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

            for (int i = 0; i < _layerPlayers.Count; i++)
            {
                var layer = _layerPlayers[i];

                if (layer.IsPlaying)
                {
                    layer.Update(deltaTime);
                    _layerPlayers[i] = layer;

                    float weight = Mathf.MoveTowards(_layerMixer.GetInputWeight(i), 1, deltaTime / Mathf.Max(layer.PlayInfo.blendTime, 0.00001f));
                    _layerMixer.SetInputWeight(i, weight);
                }
                else if (i > 0)
                {
                    _layerMixer.SetInputWeight(i, Mathf.MoveTowards(_layerMixer.GetInputWeight(i), 0, deltaTime / AnimationPlayInfo.BLEND_TIME));
                }
            }
        }

        private void CreateLayer(AnimLayer layer)
        {
            var l = _layerPlayers[layer] = new LayerPlayer(_graph);
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
            ClearQueue(animation.layer);
            Queue(animation, info);
        }

        public void Queue<TAnimation>(TAnimation animation) where TAnimation : IAnimation
        {
            Queue(animation, AnimationPlayInfo.Default);
        }

        public void Queue<TAnimation>(TAnimation animation, AnimationPlayInfo info) where TAnimation : IAnimation
        {
            if (!_layerPlayers.Contains(animation.layer)) CreateLayer(animation.layer);

            LayerPlayer player = _layerPlayers[animation.layer];

            player.Queue(UAnimation.ConstructFrom(animation), info);

            _layerPlayers[animation.layer] = player;

            _output.SetSourcePlayable(_layerMixer);

            _graph.Play();
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