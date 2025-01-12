#nullable enable
using RuniEngine.Booting;
using RuniEngine.Tasks;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RuniEngine.Tasks
{
    public class AsyncTask : ProgressTask
    {
        public static event Action? asyncTaskAdd = null;
        public static event Action? asyncTaskChange = null;
        public static event Action? asyncTaskRemove = null;



        static readonly List<AsyncTask> _asyncTasks = new List<AsyncTask>();
        public static IReadOnlyList<AsyncTask> asyncTasks => _asyncTasks;



        public static void AllAsyncTaskCancel()
        {
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

        public AsyncTask(NameSpacePathReplacePair name, NameSpacePathReplacePair description, bool isLooping = false, bool cancellable = false)
        {
            this.name = name;
            this.description = description;
            this.isLooping = isLooping;
            this.cancellable = cancellable;

            _asyncTasks.Add(this);

            EventUtility.EventInvoke(asyncTaskAdd);
            EventUtility.EventInvoke(asyncTaskChange);

            runningTimeWatch.Start();
            progressTimeWatch.Start();

            Debug.Log($"{name} async task created");
        }



        public override NameSpacePathReplacePair name { get; set; } = "";
        public override NameSpacePathReplacePair description { get; set; } = "";

        public override float progress
        {
            get => _progress;
            set
            {
                if (progress != value)
                {
                    _progress = value;
                    progressTimeWatch.Restart();
                }
            }
        }
        float _progress = 0;

        public override bool isLooping { get; set; } = false;
        public override bool cancellable { get; set; } = false;


        public override event Action? cancelEvent = null;

        public override bool isDisposed { get; protected set; } = false;

        public override float runningTime => (float)runningTimeWatch.Elapsed.TotalSeconds;
        readonly Stopwatch runningTimeWatch = new Stopwatch();

        public override float progressTime => (float)progressTimeWatch.Elapsed.TotalSeconds;
        readonly Stopwatch progressTimeWatch = new Stopwatch();



        readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        public CancellationToken cancelToken => cancelTokenSource.Token;



        public override void Dispose()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            EventUtility.EventInvoke(cancelEvent);

            _asyncTasks.Remove(this);

            EventUtility.EventInvoke(asyncTaskChange);
            EventUtility.EventInvoke(asyncTaskRemove);

            cancelTokenSource.Cancel();

            Debug.Log($"{name} async task ended");

            isDisposed = true;

            progressTimeWatch.Stop();
            runningTimeWatch.Stop();
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
