using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moths.Animations
{
    public class OnAnimatorIKInvoker : MonoBehaviour
    {
        [SerializeField] Transform root;

        private List<IOnAnimatorIKListener> _listeners = new List<IOnAnimatorIKListener>();

        public Dictionary<int, bool> layerFootIKEnable = new Dictionary<int, bool>();

        private void Reset()
        {
            root = transform.parent;
        }

        // Start is called before the first frame update
        void Awake()
        {
            _listeners = root.GetComponentsInChildren<IOnAnimatorIKListener>().ToList();
        }

        // Update is called once per frame
        void OnAnimatorIK(int layerIndex)
        {
            bool canInvoke = true;

            foreach (var enable in layerFootIKEnable)
            {
                if (enable.Value == false)
                {
                    canInvoke = false;
                    break;
                }
            }

            if (!canInvoke) return;

            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnAnimatorIKHandle(layerIndex);
            }
        }
    }
}