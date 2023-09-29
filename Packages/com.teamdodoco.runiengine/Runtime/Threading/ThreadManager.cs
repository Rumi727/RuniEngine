#nullable enable
using RuniEngine.Resource;
using System.Threading;

namespace RuniEngine.Threading
{
    public static class ThreadManager
    {
        /// <summary>
        /// { get; } = Thread.CurrentThread.ManagedThreadId
        /// </summary>
        public static int mainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// { get { mainThreadId == Thread.CurrentThread.ManagedThreadId; } }
        /// </summary>
        public static bool isMainThread => mainThreadId == Thread.CurrentThread.ManagedThreadId;

        public static void Lock(ref int location)
        {
            Debug.ForceLog(LanguageLoader.TryGetText("thread_manager.lock", Debug.NameOfCallingClass()));

            while (Interlocked.CompareExchange(ref location, 1, 0) == 0)
                Thread.Sleep(1);
        }

        public static void Unlock(ref int location)
        {
            Debug.ForceLog(LanguageLoader.TryGetText("thread_manager.unlock").Replace("{class}", Debug.NameOfCallingClass()));
            Interlocked.Exchange(ref location, 0);
        }
    }
}
