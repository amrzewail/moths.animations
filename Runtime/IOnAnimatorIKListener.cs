using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations
{
    public interface IOnAnimatorIKListener
    {
        void OnAnimatorIKHandle(int layerIndex);
    }
}