#nullable enable
using RuniEngine.Resource.Texts;
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

        public NotMainThreadException() : base(LanguageLoader.TryGetText("exception.not_main_thread"))
        {

        }
    }
}
