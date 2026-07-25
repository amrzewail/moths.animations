using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moths.Animations
{
    public class OnAnimatorIKInvoker : MonoBehaviour
    {
        [SerializeField] Transform root;

        private List<IAnimatorIKListener> _listeners = new List<IAnimatorIKListener>();

        public Dictionary<int, bool> layerFootIKEnable = new Dictionary<int, bool>();

        private void Reset()
        {
            root = transform.parent;
        }

        void Awake()
        {
            _listeners = root.GetComponentsInChildren<IAnimatorIKListener>().ToList();
        }

        void OnAnimatorIK(int layerIndex)
        {
            bool footIK = true;

            foreach (var enable in layerFootIKEnable)
            {
                if (enable.Value == false)
                {
                    footIK = false;
                    break;
                }
            }

            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnAnimatorIKHandle(layerIndex, footIK);
            }
        }
    }
}