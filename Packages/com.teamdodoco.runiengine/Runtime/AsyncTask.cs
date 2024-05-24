#nullable enable
using RuniEngine.Booting;
using RuniEngine.Resource.Texts;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RuniEngine
{
    public class AsyncTask : IProgress<float>, IDisposable
    {
        public static event Action? asyncTaskAdd = null;
        public static event Action? asyncTaskChange = null;
        public static event Action? asyncTaskRemove = null;

        public static void AsyncTaskAddEventInvoke() => asyncTaskAdd?.Invoke();
        public static void AsyncTaskRemoveEventInvoke() => asyncTaskRemove?.Invoke();
        public static void AsyncTaskChangeEventInvoke() => asyncTaskChange?.Invoke();



        static readonly List<AsyncTask> _asyncTasks = new List<AsyncTask>();
        public static IReadOnlyList<AsyncTask> asyncTasks => _asyncTasks;



        public static void AllAsyncTaskCancel()
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

            for (int i = 0; i < asyncTasks.Count; i++)
            {
                AsyncTask asyncTask = asyncTasks[i];
                if (asyncTask.cancellable)
                {
                    asyncTask.Cancel();
                    i--;
                }
            }
        }



        public AsyncTask() : this("", "", false, false) { }

        public AsyncTask(NameSpacePathReplacePair name) : this(name, "", false, false) { }

        public AsyncTask(NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool isLoop = false, bool cancellable = false, bool isEditor = false)
        {
            this.name = name;
            this.info = info;
            this.isLoop = isLoop;
            this.cancellable = cancellable;
            this.isEditor = isEditor;

            _asyncTasks.Add(this);

            AsyncTaskAddEventInvoke();
            AsyncTaskChangeEventInvoke();

            Debug.Log($"{LanguageLoader.TryGetText(name.path, name.nameSpace)} async task created");
        }



        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual NameSpacePathReplacePair name
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref nameLock);
                NameSpacePathReplacePair value = _name;
                ThreadTask.Unlock(ref nameLock);

                return value;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref nameLock);
                _name = value;
                ThreadTask.Unlock(ref nameLock);
            }
        }
        NameSpacePathReplacePair _name = "";
        int nameLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual NameSpacePathReplacePair info
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref infoLock);
                NameSpacePathReplacePair value = _info;
                ThreadTask.Unlock(ref infoLock);

                return value;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref infoLock);
                _info = value;
                ThreadTask.Unlock(ref infoLock);
            }
        }
        NameSpacePathReplacePair _info = "";
        int infoLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual bool isLoop
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return Interlocked.Add(ref _isLoop, 0) != 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                Interlocked.Exchange(ref _isLoop, value ? 1 : 0);
            }
        }
        int _isLoop = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual bool cancellable
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return Interlocked.Add(ref _cancellable, 0) != 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                Interlocked.Exchange(ref _cancellable, value ? 1 : 0);
            }
        }
        int _cancellable = 0;



        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual float progress
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return Interlocked.CompareExchange(ref _progress, 0, 0);
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                Interlocked.Exchange(ref _progress, value);
            }
        }
        float _progress = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual float maxProgress
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return Interlocked.CompareExchange(ref _maxProgress, 0, 0);
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                Interlocked.Exchange(ref _maxProgress, value);
            }
        }
        float _maxProgress = 0;


        public bool isEditor { get; } = false;



        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual bool isDisposed
        {
            get => Interlocked.Add(ref _isDisposed, 0) != 0;
            protected set => Interlocked.Exchange(ref _isDisposed, value ? 1 : 0);
        }
        int _isDisposed = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual bool isCanceled => isDisposed;



        /// <summary>
        /// Thread-Safe, cancelEvent += () => { cancelEvent += () => { }; }; Do not add more methods to this event from inside this event method like this. This causes deadlock
        /// </summary>
        public event Action? cancelEvent
        {
            add
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref cancelEventLock);
                _cancelEvent += value;
                ThreadTask.Unlock(ref cancelEventLock);
            }
            remove
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                ThreadTask.Lock(ref cancelEventLock);
                _cancelEvent -= value;
                ThreadTask.Unlock(ref cancelEventLock);
            }
        }
        event Action? _cancelEvent;
        int cancelEventLock = 0;



        readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        public CancellationToken cancelToken => cancelTokenSource.Token;



        void IProgress<float>.Report(float value)
        {
            progress = value;
            maxProgress = 1;
        }

        public virtual void Dispose()
        {
            try
            {
                ThreadTask.Lock(ref cancelEventLock);
                _cancelEvent?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                ThreadTask.Unlock(ref cancelEventLock);
            }


            _asyncTasks.Remove(this);

            AsyncTaskChangeEventInvoke();
            AsyncTaskRemoveEventInvoke();

            cancelTokenSource.Cancel();

            Debug.Log($"{LanguageLoader.TryGetText(name.path, name.nameSpace)} async task ended");

            isDisposed = true;
        }

        public bool TryDispose()
        {
            if (isDisposed || cancellable)
                return false;

            Dispose();
            return true;
        }

        public void Cancel() => Dispose();
        public bool TryCancel() => TryDispose();
    }
}
