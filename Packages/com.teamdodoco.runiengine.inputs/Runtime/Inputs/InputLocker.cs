using System;
using System.Collections.Generic;

namespace RuniEngine
{
    public sealed class InputLocker : IComparable, IComparable<InputLocker>, IDisposable
    {
        public static IReadOnlyList<InputLocker> instances => _instances;
        [StaticResettable] static readonly List<InputLocker> _instances = new List<InputLocker>();

        public static IReadOnlyList<InputLocker> oppositeInstances => _oppositeInstances;
        [StaticResettable] static readonly List<InputLocker> _oppositeInstances = new List<InputLocker>();

        public InputLocker(bool opposite = false) : this(0, opposite) { }

        public InputLocker(int priority, bool opposite = false)
        {
            _priority = priority;

            if (opposite)
            {
                _oppositeInstances.Add(this);
                _oppositeInstances.Sort();
            }
            else
            {
                _instances.Add(this);
                _instances.Sort();
            }
        }

        public int priority
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _priority;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _priority = value;
            }
        }
        int _priority = 0;

        public bool opposite
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _opposite;
            }
        }
        readonly bool _opposite;



        public int CompareTo(object value)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            return priority.CompareTo(value);
        }

        public int CompareTo(InputLocker other)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            return priority.CompareTo(other);
        }

        public bool isDisposed { get; private set; }
        public void Dispose()
        {
            isDisposed = true;
            _instances.Remove(this);
        }

        public static bool operator <(InputLocker left, InputLocker right) => left.priority < right.priority;
        public static bool operator >(InputLocker left, InputLocker right) => left.priority > right.priority;

        public static bool operator <=(InputLocker left, InputLocker right) => left.priority <= right.priority;
        public static bool operator >=(InputLocker left, InputLocker right) => left.priority >= right.priority;

        public static bool operator <(InputLocker left, int right) => left.priority < right;
        public static bool operator >(InputLocker left, int right) => left.priority > right;

        public static bool operator <=(InputLocker left, int right) => left.priority <= right;
        public static bool operator >=(InputLocker left, int right) => left.priority >= right;

        public static bool operator <(int left, InputLocker right) => left < right.priority;
        public static bool operator >(int left, InputLocker right) => left > right.priority;

        public static bool operator <=(int left, InputLocker right) => left <= right.priority;
        public static bool operator >=(int left, InputLocker right) => left >= right.priority;
    }
}
