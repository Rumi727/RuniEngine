#nullable enable
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

        public NotPlayModeException() : base("플레이 모드가 아니면 작업을 진행할 수 없습니다")
        {
            
        }
    }
}
