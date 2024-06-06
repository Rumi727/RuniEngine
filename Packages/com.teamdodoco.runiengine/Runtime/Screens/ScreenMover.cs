#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Screens
{
    public sealed class ScreenMover : IDisposable
    {
        public static IReadOnlyList<ScreenMover> instances => _instances;
        static readonly List<ScreenMover> _instances = new();

        public ScreenMover() => _instances.Add(this);

        public Vector3 position
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _position;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _position = value;
            }
        }
        Vector3 _position = Vector3.zero;



        public bool isDisposed { get; private set; }
        public void Dispose()
        {
            isDisposed = true;
            _instances.Remove(this);
        }
    }
}
