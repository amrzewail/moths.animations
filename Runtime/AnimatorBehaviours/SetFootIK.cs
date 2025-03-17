using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anima.Behaviours
{
    public class SetFootIK : StateMachineBehaviour
    {
        public bool applyFootIK = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnAnimatorIKInvoker ikInvoker = animator.GetComponent<OnAnimatorIKInvoker>();
            ikInvoker.layerFootIKEnable[layerIndex] = applyFootIK;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}