#nullable enable
using System;

namespace RuniEngine.Tasks
{
    public abstract class ProgressTask : IProgress<float>, IDisposable
    {
        public abstract NameSpacePathReplacePair name { get; set; }
        public abstract NameSpacePathReplacePair description { get; set; }

        public abstract float progress { get; set; }

        public abstract bool isLooping { get; set; }
        public abstract bool cancellable { get; set; }

        public bool isRunning => !isDisposed;

        public abstract event Action? cancelEvent;

        public abstract bool isDisposed { get; protected set; }
        public bool isCanceled => isDisposed;

        public abstract float runningTime { get; }
        public abstract float progressTime { get; }

        void IProgress<float>.Report(float value) => progress = value;

        public abstract void Dispose();
    }
}
