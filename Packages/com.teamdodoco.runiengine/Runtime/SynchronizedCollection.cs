#nullable enable
//Source: https://referencesource.microsoft.com/#System.ServiceModel/System/ServiceModel/SynchronizedCollection.cs
using System.Threading;

namespace System.Collections.Generic
{
    [Runtime.InteropServices.ComVisible(false)]
    public class SynchronizedCollection<T> : IList<T?>, IList
    {
        public readonly List<T?> internalList;
        public int internalSync = 0;

        public SynchronizedCollection() => internalList = new List<T?>();

        public SynchronizedCollection(IEnumerable<T?>? list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            internalList = new List<T?>(list);
        }

        public SynchronizedCollection(params T?[]? list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            internalList = new List<T?>(list);
        }

        public int Count
        {
            get
            {
                while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                    Thread.Yield();

                int count = internalList.Count;

                Interlocked.Decrement(ref internalSync);

                return count;
            }
        }

        protected List<T?> Items => internalList;

        public T? this[int index]
        {
            get
            {
                while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                    Thread.Yield();

                try
                {
                    return internalList[index];
                }
                finally
                {
                    Interlocked.Decrement(ref internalSync);
                }
            }
            set
            {
                while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                    Thread.Yield();

                try
                {
                    if (index < 0 || index >= internalList.Count)
                        throw new ArgumentOutOfRangeException();

                    SetItem(index, value);
                }
                finally
                {
                    Interlocked.Decrement(ref internalSync);
                }
            }
        }

        public void Add(T? item)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                int index = internalList.Count;
                InsertItem(index, item);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public void Clear()
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                ClearItems();
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public void CopyTo(T?[]? array, int index)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                internalList.CopyTo(array, index);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public bool Contains(T? item)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                return internalList.Contains(item);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public IEnumerator<T?> GetEnumerator()
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                return internalList.GetEnumerator();
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public int IndexOf(T? item)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                return InternalIndexOf(item);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public void Insert(int index, T? item)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                if (index < 0 || index > internalList.Count)
                    throw new ArgumentOutOfRangeException();

                InsertItem(index, item);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        int InternalIndexOf(T? item)
        {
            int count = internalList.Count;

            for (int i = 0; i < count; i++)
            {
                if (Equals(internalList[i], item))
                    return i;
            }
            return -1;
        }

        public bool Remove(T? item)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                int index = InternalIndexOf(item);
                if (index < 0)
                    return false;

                RemoveItem(index);
                return true;
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        public void RemoveAt(int index)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                if (index < 0 || index >= internalList.Count)
                    throw new ArgumentOutOfRangeException();

                RemoveItem(index);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        protected virtual void ClearItems() => internalList.Clear();

        protected virtual void InsertItem(int index, T? item) => internalList.Insert(index, item);

        protected virtual void RemoveItem(int index) => internalList.RemoveAt(index);

        protected virtual void SetItem(int index, T? item) => internalList[index] = item;

        bool ICollection<T?>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => ((IList)internalList).GetEnumerator();

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => internalSync;

        void ICollection.CopyTo(Array? array, int index)
        {
            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                ((IList)internalList).CopyTo(array, index);
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                VerifyValueType(value);
                this[index] = (T?)value;
            }
        }

        bool IList.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        int IList.Add(object? value)
        {
            VerifyValueType(value);

            while (Interlocked.CompareExchange(ref internalSync, 1, 0) != 0)
                Thread.Yield();

            try
            {
                Add((T?)value);
                return Count - 1;
            }
            finally
            {
                Interlocked.Decrement(ref internalSync);
            }
        }

        bool IList.Contains(object? value)
        {
            VerifyValueType(value);
            return Contains((T?)value);
        }

        int IList.IndexOf(object? value)
        {
            VerifyValueType(value);
            return IndexOf((T?)value);
        }

        void IList.Insert(int index, object? value)
        {
            VerifyValueType(value);
            Insert(index, (T?)value);
        }

        void IList.Remove(object? value)
        {
            VerifyValueType(value);
            Remove((T?)value);
        }

        static void VerifyValueType(object? value)
        {
            if (value == null)
            {
                if (typeof(T).IsValueType)
                    throw new ArgumentException();
            }
            else if (value is not T)
                throw new ArgumentException();
        }
    }
}