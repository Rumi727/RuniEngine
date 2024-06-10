#nullable enable
using System;
using System.Collections.Generic;

namespace RuniEngine
{
    public sealed class InputLocker : IComparable<InputLocker>, IDisposable
    {
        public static IReadOnlyList<InputLocker> instances => _instances;
        static readonly List<InputLocker> _instances = new List<InputLocker>();

        public InputLocker() : this(0) { }

        public InputLocker(int priority)
        {
            _priority = priority;

            _instances.Add(this);
            _instances.Sort();
        }

        public int priority
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _priority;
            }
        }
        readonly int _priority = 0;


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



        public static bool operator <(InputLocker left, InputLocker right) => left.priority <= right.priority;
        public static bool operator >(InputLocker left, InputLocker right) => left.priority >= right.priority;

        public static bool operator <=(InputLocker left, InputLocker right) => left.priority <= right.priority;
        public static bool operator >=(InputLocker left, InputLocker right) => left.priority >= right.priority;

        public static bool operator <(InputLocker left, int right) => left.priority <= right;
        public static bool operator >(InputLocker left, int right) => left.priority >= right;

        public static bool operator <=(InputLocker left, int right) => left.priority <= right;
        public static bool operator >=(InputLocker left, int right) => left.priority >= right;

        public static bool operator <(int left, InputLocker right) => left <= right.priority;
        public static bool operator >(int left, InputLocker right) => left >= right.priority;

        public static bool operator <=(int left, InputLocker right) => left <= right.priority;
        public static bool operator >=(int left, InputLocker right) => left >= right.priority;
    }
}
