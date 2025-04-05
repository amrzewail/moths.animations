using UnityEngine;

namespace Moths.Animations.Collections
{
    public struct PlayingLayersMask
    {
        private int _mask;

        public void Play(uint layer) => _mask |= 1 << (int)layer;

        public void Stop(uint layer) => _mask &= ~1 << (int)layer;

        public bool IsPlaying(uint layer) => (_mask & (1 << (int)layer)) == 1 << (int)layer;
    }
}