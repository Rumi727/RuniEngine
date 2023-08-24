#nullable enable
using System;

namespace RuniEngine.Threading
{
    public class NotMainThreadException : Exception
    {
        public static void Exception()
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadException();
        }

        public NotMainThreadException() : base("메인 스레드에서만 작업을 진행할 수 있습니다")
        {

        }
    }
}
