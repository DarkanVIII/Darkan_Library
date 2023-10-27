using System.Collections.Generic;
using UnityEngine;

namespace Darkan.ObjectPooling
{
    public class SimpleObjectPool<T> where T : MonoBehaviour
    {
        public SimpleObjectPool(T prefab, int prefill = 0, bool getActivatedObjs = true)
        {
            _prefab = prefab;
            _getActivatedObjs = getActivatedObjs;

            _pool = new(prefill);
            PrefillPool(prefill);
        }



        readonly bool _getActivatedObjs;
        readonly T _prefab;
        readonly Stack<T> _pool;
        int _countAll;

        public int CountInactive => _pool.Count;
        public int CountActive => _countAll - _pool.Count;
        public int CountAll => _countAll;

        public void Clear()
        {
            foreach (var item in _pool)
            {
                Object.Destroy(item);
            }

            _pool.Clear();
            _countAll = 0;
        }

        void PrefillPool(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var obj = Object.Instantiate(_prefab);
                obj.gameObject.SetActive(false);
                _pool.Push(obj);
                _countAll++;
            }
        }

        T CreateNewObject()
        {
            _countAll++;

            T obj = Object.Instantiate(_prefab);
            obj.GetComponent<IPooled<T>>().OnReturnToPool += Release;
            return obj;
        }

        public T Take()
        {
            T obj;

            if (_pool.Count == 0)
            {
                obj = CreateNewObject();
                obj.gameObject.SetActive(_getActivatedObjs);
                return obj;
            }
            else
            {
                obj = _pool.Pop();
                obj.gameObject.SetActive(_getActivatedObjs);
                return obj;
            }
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
        }
    }
}
