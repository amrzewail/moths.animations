using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moths.Animations.Samples
{
    public class PlayablesTest : MonoBehaviour
    {
        [SerializeField] AnimationClip clip;

        PlayableGraph _graph;

        private IEnumerator Start()
        {
            var animator = GetComponent<Animator>();

            yield return new WaitForSeconds(1);

            _graph = PlayableGraph.Create();
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var clipPlayable = AnimationClipPlayable.Create(_graph, clip);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Output", animator);

            output.SetSourcePlayable(clipPlayable);

            _graph.Play();


            yield return new WaitForSeconds(3);

            clipPlayable.SetTime(0);
        }


        private void OnDestroy()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
        }
    }
}