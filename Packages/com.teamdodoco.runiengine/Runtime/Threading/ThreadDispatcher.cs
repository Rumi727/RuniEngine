#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System;
using RuniEngine.Booting;
using System.Diagnostics;
using UnityEngine;

namespace RuniEngine.Threading
{
    public static class ThreadDispatcher
    {
        public const int allotedTime = 3;

        [Awaken]
        public static void Awaken()
        {
            CustomPlayerLoopSetter.updateEvent += Update;
            Application.quitting += Quit;
        }

        static readonly Stopwatch stopwatch = new Stopwatch();
        static readonly ConcurrentQueue<Action> scheduledTasks = new ConcurrentQueue<Action>();
        public static void Update()
        {
            stopwatch.Restart();

            while (scheduledTasks.TryDequeue(out Action action) && stopwatch.Elapsed.TotalMilliseconds < allotedTime)
                action();
        }

        public static void Quit()
        {
            while (scheduledTasks.TryDequeue(out Action action))
                action();
        }

        public static UniTask<T> Execute<T>(Func<T> func)
        {
            UniTaskCompletionSource<T> tcs = new UniTaskCompletionSource<T>();
            void InternalAction()
            {
                try
                {
                    T returnValue = func();
                    tcs.TrySetResult(returnValue);
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
            }

            scheduledTasks.Enqueue(InternalAction);
            return tcs.Task;
        }

        public static UniTask Execute(Action action)
        {
            UniTaskCompletionSource tcs = new UniTaskCompletionSource();
            void InternalAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult();
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
            }

            scheduledTasks.Enqueue(InternalAction);
            return tcs.Task;
        }
    }
}
