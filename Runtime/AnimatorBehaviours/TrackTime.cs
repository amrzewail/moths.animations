using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations.Behaviours
{
    public class TrackTime : StateMachineBehaviour
    {

        private IAnimator _controller;
        private bool _invokedFinish = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _invokedFinish = false;

            _controller = animator.GetComponent<IAnimator>();

            var animation = _controller.GetCurrentAnimation(layerIndex);
            if (animation == null) return;
            if (!stateInfo.IsName(animation.stateName)) return;

            _controller.SetNormalizedTime(layerIndex, 0);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animation = _controller.GetCurrentAnimation(layerIndex);
            if (animation == null) return;
            if (!stateInfo.IsName(animation.stateName)) return;

            float time = stateInfo.loop ? stateInfo.normalizedTime % 1.0f : Mathf.Clamp01(stateInfo.normalizedTime);
            if (_controller != null && time <= 1)
            {
                if (time < 1 || stateInfo.loop)
                {
                    _controller.SetNormalizedTime(layerIndex, time);
                }
                else if (time == 1 && !_invokedFinish)
                {
                    _controller.SetNormalizedTime(layerIndex, time);
                    _invokedFinish = true;
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _invokedFinish = false;
        }
    }
}