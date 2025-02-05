#nullable enable
using RuniEngine.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RuniEngine.Threading
{
    public class ThreadTask : ProgressTask
    {
        /// <summary>
        /// { get; } = Thread.CurrentThread.ManagedThreadId
        /// </summary>
        public static int mainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// { get { mainThreadId == Thread.CurrentThread.ManagedThreadId; } }
        /// </summary>
        public static bool isMainThread => mainThreadId == Thread.CurrentThread.ManagedThreadId;


        public static SynchronizedCollection<ThreadTask?> runningThreads { get; } = new();



        public static void Lock(ref int location)
        {
            while (Interlocked.CompareExchange(ref location, 1, 0) != 0)
            {
                if (InfiniteLoopDetector.RunWithoutException())
                {
                    Debug.LogError("[SpinLock] Deadlock Detected!!!", Debug.NameOfCallingClass());
                    return;
                }

                Thread.Yield();
            }
        }

        public static void Unlock(ref int location)
        {
            Interlocked.Exchange(ref location, 0);
        }



        #region 이벤트
        public static event Action? threadAdd
        {
            add
            {
                lock (threadAddLock)
                {
                    _threadAdd += value;
                }
            }
            remove
            {
                lock (threadAddLock)
                {
                    _threadAdd -= value;
                }
            }
        }
        static Action? _threadAdd;
        static readonly object threadAddLock = new();

        public static event Action? threadChange
        {
            add
            {
                lock (threadChangeLock)
                {
                    _threadChange += value;
                }
            }
            remove
            {
                lock (threadChangeLock)
                {
                    _threadChange -= value;
                }
            }
        }
        static Action? _threadChange;
        static readonly object threadChangeLock = new();

        public static event Action? threadRemove
        {
            add
            {
                lock (threadRemoveLock)
                {
                    _threadRemove += value;
                }
            }
            remove
            {
                lock (threadRemoveLock)
                {
                    _threadRemove -= value;
                }
            }
        }
        static Action? _threadRemove;
        static readonly object threadRemoveLock = new();

        static void ThreadAddEventInvoke()
        {
            lock (threadAddLock)
            {
                EventUtility.EventInvoke(_threadAdd);
            }
            ThreadChangeEventInvoke();
        }

        static void ThreadChangeEventInvoke()
        {
            lock (threadChangeLock)
            {
                EventUtility.EventInvoke(_threadChange);
            }
        }

        static void ThreadRemoveEventInvoke()
        {
            ThreadChangeEventInvoke();
            lock (threadRemoveLock)
            {
                EventUtility.EventInvoke(_threadRemove);
            }
        }
        #endregion



        public override NameSpacePathReplacePair name
        {
            get
            {
                lock (nameLock)
                {
                    return _name;
                }
            }
            set
            {
                lock (nameLock)
                {
                    _name = value;
                }
            }
        }
        NameSpacePathReplacePair _name = "";
        public readonly object nameLock = new();

        public override NameSpacePathReplacePair description
        {
            get
            {
                lock (descriptionLock)
                {
                    return _description;
                }
            }
            set
            {
                lock (descriptionLock)
                {
                    _description = value;
                }
            }
        }
        NameSpacePathReplacePair _description = "";
        public readonly object descriptionLock = new();

        public override float progress
        {
            get => Interlocked.CompareExchange(ref _progress, 0, 0);
            set
            {
                if (Interlocked.Exchange(ref _progress, value) != value)
                    progressTimeWatch.Restart();
            }
        }
        float _progress = 0;

        public override bool isLooping
        {
            get => Interlocked.Add(ref _isLooping, 0) != 0;
            set => Interlocked.Exchange(ref _isLooping, value ? 1 : 0);
        }
        int _isLooping = 0;

        public override bool cancellable
        {
            get => Interlocked.Add(ref _cancellable, 0) != 0;
            set => Interlocked.Exchange(ref _cancellable, value ? 1 : 0);
        }
        int _cancellable = 0;

        public override event Action? cancelEvent
        {
            add
            {
                lock (cancelEventLock)
                {
                    _cancelEvent += value;
                }
            }
            remove
            {
                lock (cancelEventLock)
                {
                    _cancelEvent -= value;
                }
            }
        }
        Action? _cancelEvent = null;
        public readonly object cancelEventLock = new();

        public override bool isDisposed
        {
            get => Interlocked.Add(ref _isDisposed, 0) != 0;
            protected set => Interlocked.Exchange(ref _isDisposed, value ? 1 : 0);
        }
        int _isDisposed = 0;

        public override float runningTime
        {
            get
            {
                try
                {
                    Lock(ref stopwatchLock);
                    return (float)runningTimeWatch.Elapsed.TotalSeconds;
                }
                finally
                {
                    Unlock(ref stopwatchLock);
                }
            }
        }
        readonly Stopwatch runningTimeWatch = new Stopwatch();
        int stopwatchLock = 0;

        public override float progressTime
        {
            get
            {
                try
                {
                    Lock(ref stopwatchLock);
                    return (float)progressTimeWatch.Elapsed.TotalSeconds;
                }
                finally
                {
                    Unlock(ref stopwatchLock);
                }
            }
        }
        readonly Stopwatch progressTimeWatch = new Stopwatch();






        #region 생성자
        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method) : this(method, "", "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method, NameSpacePathReplacePair name) : this(method, name, "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method, NameSpacePathReplacePair name, NameSpacePathReplacePair description, bool isLooping = false, bool cancellable = false)
        {
            this.name = name;
            this.description = description;
            this.isLooping = isLooping;
            this.cancellable = cancellable;

            runningThreads.Add(this);
            ThreadAddEventInvoke();

            runningTimeWatch.Start();
            progressTimeWatch.Start();

            thread = new Thread(method);
            thread.Start();

            Debug.Log($"{name} thread task created");
        }
        #endregion



        public Thread? thread { get; private set; } = null;

        public override void Dispose()
        {
            Debug.ForceLog($"{name} Thread Remove! Beware the Join method");

            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            EventUtility.EventInvoke(_cancelEvent);

            runningThreads.Remove(this);

            EventUtility.EventInvoke(_threadChange);
            EventUtility.EventInvoke(_threadRemove);

            Debug.Log($"{name} async task ended");

            isDisposed = true;

            progressTimeWatch.Stop();
            runningTimeWatch.Stop();
        }
    }
}
