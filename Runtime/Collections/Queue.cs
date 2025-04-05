using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations.Collections
{
    public struct Queue<T>
    {

        private List<T> _list;
        private int _current;

        public T Current => IsFinished ? default(T) : _list[_current];

        public bool IsFinished => _current >= _list.Count;

        public int Count => _list.Count;

        public Queue(int capacity)
        {
            this = default;
            _list = new List<T>(capacity);
        }

        public void Next()
        {
            if (IsFinished) return;
            _current++;
        }

        public T PeekNext() => _list[_current + 1];

        public bool HasNext() => _current < _list.Count - 1;

        public void Add(T element)
        {
            if (_list == null) return;
            _list.Add(element);
        }

        public void Clear()
        {
            if (_list == null) return;
            _current = 0;
            _list.Clear();
        }

        public void Restart() => _current = 0;

    }
}