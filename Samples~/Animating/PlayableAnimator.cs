
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


namespace Moths.Animations.Samples
{


    public class PlayableAnimator : MonoBehaviour
    {
        private IAnimator _animator;

        [SerializeField] AnimationReference _idle;
        [SerializeField] AnimationReference _walkCircle;
        [SerializeField] AnimationReference _punch;
        [SerializeField] AnimationReference _jump;

        //public Animator animator;
        //public AnimationClip baseAnimation;   // Example: Running animation

        //private PlayableGraph playableGraph;
        //private AnimationLayerMixerPlayable layerMixer;

        void Start()
        {
            //// Create Playable Graph
            //playableGraph = PlayableGraph.Create("AnimationLayerGraph");
            //var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);

            //// Create Base Animation Playable
            //var basePlayable = AnimationClipPlayable.Create(playableGraph, baseAnimation);

            //// Create Layer Mixer Playable
            //layerMixer = AnimationLayerMixerPlayable.Create(playableGraph, 2);

            //// Connect Base Animation (Layer 0)
            //playableGraph.Connect(basePlayable, 0, layerMixer, 0);
            //layerMixer.SetInputWeight(0, 1f); // Always play base animation fully

            //// Set Output
            //playableOutput.SetSourcePlayable(layerMixer);

            //// Play Graph
            //playableGraph.Play();

            _animator = GetComponent<IAnimator>();
        }

        void Update()
        {

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