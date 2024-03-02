#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Screens
{
    public sealed class ScreenCroper : IDisposable
    {
        public static List<ScreenCroper> instances { get; } = new List<ScreenCroper>();

        public ScreenCroper() => instances.Add(this);

        public RectOffset offset
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                RectOffset result = _offset;

                result.min = result.min;
                result.max = result.max;

                return result;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _offset = value;
            }
        }
        RectOffset _offset = RectOffset.zero;



        public bool isDisposed { get; private set; }
        public void Dispose()
        {
            isDisposed = true;
            instances.Remove(this);
        }
    }
}
