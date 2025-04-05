using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations.Collections
{
    [System.Serializable]
    public struct AnimLayer
    {
        [SerializeField] AvatarMask _mask;

        public AvatarMask Mask => _mask;

        public AnimLayer(AvatarMask mask)
        {
            _mask = mask;
        }

        public override bool Equals(object obj)
        {
            return obj is AnimLayer layer && _mask == layer._mask;
        }

        //public static implicit operator int(AnimLayer layer) => layer.GetHashCode();

        public override int GetHashCode() => _mask ? _mask.GetHashCode() : 0;

        public static bool operator ==(AnimLayer lhs, AnimLayer rhs) => lhs._mask == rhs._mask;
        public static bool operator !=(AnimLayer lhs, AnimLayer rhs) => lhs._mask != rhs._mask;

    }
}