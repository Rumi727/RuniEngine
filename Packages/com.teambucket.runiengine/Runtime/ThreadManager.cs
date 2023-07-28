#nullable enable
using System.Threading;

namespace RuniEngine
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
    }
}
