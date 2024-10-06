using RuniEngine.Resource.Texts;
using System;
using System.Diagnostics;

namespace RuniEngine.Booting
{
    public class NotPlayModeException : Exception
    {
        [Conditional("UNITY_EDITOR")]
        public static void Exception()
        {
            if (!Kernel.isPlaying)
                throw new NotPlayModeException();
        }

        public NotPlayModeException() : base(LanguageLoader.GetText("exception.not_play_mode"))
        {

        }
    }
}
