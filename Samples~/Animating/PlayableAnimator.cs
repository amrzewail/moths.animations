
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections;
using Cysharp.Threading.Tasks;

namespace Moths.Animations.Samples
{
    public class PlayableAnimator : MonoBehaviour
    {
        private IAnimator _animator;

        [SerializeField] AnimationReference[] _queue;

        [SerializeField] AnimationReference _idle;
        [SerializeField] AnimationReference _walkCircle;
        [SerializeField] AnimationReference _punch;
        [SerializeField] AnimationReference _jump;

        [Space]
        [SerializeField] AnimationReference _attack1;
        [SerializeField] AnimationReference _attack2;

        void Awake()
        {
            _animator = GetComponent<IAnimator>();
        }

        private async void Start()
        {

            //Animator animator = GetComponent<Animator>();

            //PlayableGraph graph = PlayableGraph.Create();

            //AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "animation", animator);

            //AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph, 1);
            //output.SetSourcePlayable(mixer);

            //graph.Play();

            //var animation = GetPlayable(graph, mixer, _attack1.clip);
            //graph.Connect(animation, 0, mixer, 0);

            //while (true)
            //{
            //    animation.SetTime(0 - Time.deltaTime);
            //    animation.SetTime(0);

            //    await UniTask.WaitForSeconds(3);
            //}
        }

        private Playable GetPlayable(PlayableGraph graph, Playable mixer, AnimationClip clip)
        {
            AnimationClipPlayable animation = AnimationClipPlayable.Create(graph, clip);
            mixer.SetInputWeight(0, 1);
            return animation;
        }

        IEnumerator StartF()
        {
            _animator.Play(_idle);

            yield return new WaitForSeconds(1);

            _animator.Play(_attack1);

            yield return new WaitForSeconds(3);

            _animator.Play(_attack1);

            yield return new WaitForSeconds(3);

            _animator.Play(_idle);
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _animator.Play(_attack1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _animator.SetNormalizedTime(_attack1.layer, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _animator.Play(_attack2);
            }

            if (Input.GetKeyDown("q"))
            {
                _animator.ClearQueue(_queue[0].layer);
                for (int i = 0; i < _queue.Length; i++) _animator.Queue(_queue[i]);
            }

            if (Input.GetKeyDown("f"))
            {
                _animator.Play(_idle.Value);
            }


            if (Input.GetKeyDown("g"))
            {
                _animator.Play(_walkCircle);
            }

            if (Input.GetKeyDown("s"))
            {
                _animator.Stop(_walkCircle.layer);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.ClearQueue(_jump.layer);
                _animator.Queue(_jump);
                _animator.Queue(_walkCircle);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _animator.Play(_punch);
            }
        }

        void OnDestroy()
        {
            // Cleanup Playable Graph
            //playableGraph.Destroy();
        }
    }

}