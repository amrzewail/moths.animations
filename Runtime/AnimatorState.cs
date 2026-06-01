using System;
using UnityEngine;

namespace Moths.Animations
{
    [System.Serializable]
    public struct AnimatorState : IEquatable<AnimatorState>
    {
        [SerializeField] AnimatorStateReferences _references;
        [SerializeField] string _guid;

        private string _stateName;
        private int _layer;

        public int Layer => _references ? _references.GetLayer(_guid) : _layer;
        public string StateName => _references ? _references.GetStateName(_guid) : _stateName;
        public float Duration => _references ? _references.GetDuration(_guid) : 0;
        public bool IsValid => !string.IsNullOrEmpty(StateName) && Duration > 0;

        public static bool operator ==(AnimatorState left, AnimatorState right) => left.Equals(right);
        public static bool operator !=(AnimatorState left, AnimatorState right) => !left.Equals(right);

        public AnimatorState(int layer, string stateName)
        {
            this = default;
            _layer = layer;
            _stateName = stateName;
        }

        public static AnimatorState StopState(int layer) => new(layer, "$__StopID__$");

        public bool Equals(AnimatorState other)
        {
            if (!IsValid || !other.IsValid) return false;
            return Layer == other.Layer && StateName == other.StateName;
        }

        public override bool Equals(object obj)
        {
            return obj is AnimatorState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_layer, _stateName);
        }
    }
}