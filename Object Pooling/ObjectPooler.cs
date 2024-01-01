using System;
using System.Collections.Generic;

namespace Darkan.Pooling
{
    public class ObjectPooler<T> where T : class
    {
        readonly Stack<T> _stack;

        readonly Func<T> _createFunc;

        readonly Action<T> _actionOnGet;

        readonly Action<T> _actionOnRelease;

        readonly Action<T> _actionOnDestroy;

#if !RELEASE
        public int CountAll { get; private set; }
#else
        public int CountAll;
#endif

        public int CountActive => CountAll - CountInactive;

        public int CountInactive => _stack.Count;

        public ObjectPooler(Func<T> createFunc, Action<T> actionOnRelease, Action<T> actionOnGet = null, Action<T> actionOnDestroy = null,
            ushort prefillAmount = 10, ushort defaultCapacity = 25)
        {
            _stack = new Stack<T>(defaultCapacity);

            _createFunc = createFunc ?? throw new ArgumentNullException("createFunc");
            _actionOnRelease = actionOnRelease ?? throw new ArgumentNullException("releaseAction");
            _actionOnGet = actionOnGet;
            _actionOnDestroy = actionOnDestroy;

            PrefillPool(prefillAmount);
        }

        void PrefillPool(ushort amount)
        {
            for (int i = 1; i <= amount; i++)
            {
                T obj = _createFunc();
                _actionOnRelease?.Invoke(obj);
                _stack.Push(obj);
                CountAll++;
            }
        }

        public T Get()
        {
            T obj;
            if (_stack.Count == 0)
            {
                obj = _createFunc();
                CountAll++;
            }
            else
            {
                obj = _stack.Pop();
            }

            _actionOnGet?.Invoke(obj);
            return obj;
        }

        public void Release(T element)
        {
            _actionOnRelease?.Invoke(element);
            _stack.Push(element);
        }

        public void Clear()
        {
            if (_actionOnDestroy != null)
            {
                foreach (T item in _stack)
                {
                    _actionOnDestroy(item);
                }
            }

            _stack.Clear();
            CountAll = 0;
        }
    }
}
