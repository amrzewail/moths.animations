using System.Collections.Generic;
using UnityEngine;

namespace Moths.Animations.Collections
{
    public struct IndexedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, int> _dictionary;
        private List<TValue> _list;


        public TValue this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public TValue this[TKey key]
        {
            get => this[_dictionary[key]];
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    _list[_dictionary[key]] = value;
                    return;
                }
                _list.Add(value);
                _dictionary[key] = _list.Count - 1;
            }
        }

        public int Count => _list.Count;
        
        public int Capacity => _list.Capacity;

        public IndexedDictionary(int count)
        {
            _dictionary = new Dictionary<TKey, int>(count);
            _list = new List<TValue>(count);
        }

        public bool Contains(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public int IndexOf(TKey key) => _dictionary[key];

        //public void Remove(TKey key)
        //{
        //    _list.RemoveAt(_dictionary[key]);
        //    _dictionary.Remove(key);
        //}

        public bool TryGetValue(TKey key, out TValue value, out int index)
        {
            if (_dictionary.TryGetValue(key, out index))
            {
                value = _list[index];
                return true;
            }
            value = default;
            index = -1;
            return false;
        }
    }

}
