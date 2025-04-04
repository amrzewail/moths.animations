using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public class CharacterAnimatorKeyframer : MonoBehaviour
    {

        [SerializeField] Animator animator;

        [SerializeField] bool keyframe = false;

        private void OnValidate()
        {
            animator = GetComponentInChildren<Animator>();

            if (keyframe)
            {
                Keyframe();
                keyframe = false;
            }
        }

        private void Keyframe()
        {
            animator.GetBoneTransform(HumanBodyBones.Head).localRotation = Quaternion.Euler(Vector3.zero);
        }

        private void OnDrawGizmosSelected()
        {
            Color color = Gizmos.color;

            Gizmos.color = Color.yellow;


            Gizmos.DrawSphere(animator.GetBoneTransform(HumanBodyBones.Head).position, 0.05f);


            Gizmos.color = color;

        }

    }
}