using Moths.Animations;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] AnimatorState _animation;

    private void Update()
    {
        GetComponent<IAnimator>().Play(_animation);
    }
}
