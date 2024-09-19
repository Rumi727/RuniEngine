#nullable enable
using RuniEngine.Resource.Texts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RuniEngine.Threading
{
    public class ThreadTask : AsyncTask
    {
        /// <summary>
        /// { get; } = Thread.CurrentThread.ManagedThreadId
        /// </summary>
        public static int mainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// { get { mainThreadId == Thread.CurrentThread.ManagedThreadId; } }
        /// </summary>
        public static bool isMainThread => mainThreadId == Thread.CurrentThread.ManagedThreadId;


        static readonly List<ThreadTask> _runningThreads = new List<ThreadTask>();
        public static IReadOnlyList<ThreadTask> runningThreads { get; } = _runningThreads;



        #region 이벤트
        public static event Action? threadAdd
        {
            add
            {
                Lock(ref threadAddLock);
                _threadAdd += value;
                Unlock(ref threadAddLock);
            }
            remove
            {
                Lock(ref threadAddLock);
                _threadAdd -= value;
                Unlock(ref threadAddLock);
            }
        }
        static Action? _threadAdd;
        static int threadAddLock = 0;

        public static event Action? threadChange
        {
            add
            {
                Lock(ref threadChangeLock);
                _threadChange += value;
                Unlock(ref threadChangeLock);
            }
            remove
            {
                Lock(ref threadChangeLock);
                _threadChange -= value;
                Unlock(ref threadChangeLock);
            }
        }
        static Action? _threadChange;
        static int threadChangeLock = 0;

        public static event Action? threadRemove
        {
            add
            {
                Lock(ref threadRemoveLock);
                _threadRemove += value;
                Unlock(ref threadRemoveLock);
            }
            remove
            {
                Lock(ref threadRemoveLock);
                _threadRemove -= value;
                Unlock(ref threadRemoveLock);
            }
        }
        static Action? _threadRemove;
        static int threadRemoveLock = 0;

        static void ThreadAddEventInvoke()
        {
            Lock(ref threadAddLock);
            _threadAdd?.Invoke();
            Unlock(ref threadAddLock);

            ThreadChangeEventInvoke();
        }

        static void ThreadChangeEventInvoke()
        {
            Lock(ref threadChangeLock);
            _threadChange?.Invoke();
            Unlock(ref threadChangeLock);
        }

        static void ThreadRemoveEventInvoke()
        {
            ThreadChangeEventInvoke();

            Lock(ref threadRemoveLock);
            _threadRemove?.Invoke();
            Unlock(ref threadRemoveLock);
        }
        #endregion



        public static void Lock(ref int location)
        {
            //Debug.ForceLog(LanguageLoader.TryGetText("thread_manager.lock").Replace("{class}", Debug.NameOfCallingClass()));
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
            //Debug.ForceLog(LanguageLoader.TryGetText("thread_manager.unlock").Replace("{class}", Debug.NameOfCallingClass()));
            Interlocked.Exchange(ref location, 0);
        }



        #region 생성자
        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method) : this(method, "", "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method, NameSpacePathReplacePair name) : this(method, name, "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(ThreadStart method, NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool loop = false, bool cancellable = false) : base(name, info, loop, cancellable)
        {
            NotMainThreadException.Exception();

            thread = new Thread(method);
            thread.Start();

            _runningThreads.Add(this);
            ThreadAddEventInvoke();

            Debug.Log($"{LanguageLoader.TryGetText(name.path, name.nameSpace)} thread meta data created");
        }



        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(Action<ThreadTask> method) : this(method, "", "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(Action<ThreadTask> method, NameSpacePathReplacePair name) : this(method, name, "", false, false) { }

        /// <exception cref="NotMainThreadException"></exception>
        public ThreadTask(Action<ThreadTask> method, NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool loop = false, bool cancellable = false) : base(name, info, loop, cancellable)
        {
            NotMainThreadException.Exception();

            thread = new Thread(() => method(this));
            thread.Start();

            _runningThreads.Add(this);
            ThreadAddEventInvoke();

            Debug.Log($"{LanguageLoader.TryGetText(name.path, name.nameSpace)} thread meta data created");
        }
        #endregion



        public Thread? thread { get; private set; } = null;

        /// <summary>
        /// 이 함수는 메인 스레드에서만 실행할수 있습니다
        /// This function can only be executed on the main thread
        /// </summary>
        /// <exception cref="NotMainThreadException"></exception>
        public override void Dispose()
        {
            NotMainThreadException.Exception();

            Debug.ForceLog($"{LanguageLoader.TryGetText(name.path, name.nameSpace)} Thread Remove! Beware the Join method");

            _runningThreads.Remove(this);
            ThreadRemoveEventInvoke();

            if (thread != null)
            {
                thread.Join();
                thread = null;
            }

            base.Dispose();
        }
    }
}
