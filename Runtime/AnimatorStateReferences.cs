using UnityEngine;
using Moths.Collections;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Moths.Animations
{
    public class AnimatorStateReferences : ScriptableObject
    {
#if UNITY_EDITOR
        public AnimatorController animator;
#endif
        [System.Serializable]
        public struct SerializedAnimatorState
        {
#if UNITY_EDITOR
            public UnityEditor.Animations.AnimatorState state;
#endif
            public int layer;
            public string name;
            public float duration;
        }

        public SerializableDictionary<long, SerializedAnimatorState> states = new();

        public int GetLayer(long id)
        {
            if (states.TryGetValue(id, out var state)) return state.layer;
            return -1;
        }

        public string GetStateName(long id)
        {
            if (states.TryGetValue(id, out var state)) return state.name;
            return string.Empty;
        }

        public float GetDuration(long id)
        {
            if (states.TryGetValue(id, out var state)) return state.duration;
            return 0;
        }
    }
}