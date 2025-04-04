using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Moths.Animations.Behaviours
{
    public class VariableSpeed : StateMachineBehaviour
    {
        public AnimationCurve speedCurve;

        private float _startSpeed;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _startSpeed = animator.speed;
            animator.speed = speedCurve.Evaluate(0);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime < 0.99f)
            {
                animator.speed = speedCurve.Evaluate(stateInfo.normalizedTime);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.speed = _startSpeed;
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
        {
            animator.speed = _startSpeed;
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}