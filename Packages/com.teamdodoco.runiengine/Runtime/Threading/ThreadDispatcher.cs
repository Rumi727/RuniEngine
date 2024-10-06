using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using System;
using System.Collections.Concurrent;
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
            Application.quitting += ForceScheduledTasksExecute;
        }

        static readonly Stopwatch stopwatch = new Stopwatch();
        static readonly ConcurrentQueue<Action> scheduledTasks = new ConcurrentQueue<Action>();
        public static void Update()
        {
            stopwatch.Restart();

            /*
             * 순서 중요!
             * 시간 초과 코드가 맨 뒤에 있을 경우 작업 리스트에서는 빠지는데 시간 초과로 인해 코드가 작동하지 않는 경우가 생김!!!
             */
            while (stopwatch.Elapsed.TotalMilliseconds < allotedTime && scheduledTasks.TryDequeue(out Action action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void ForceScheduledTasksExecute()
        {
            while (scheduledTasks.TryDequeue(out Action action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
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
