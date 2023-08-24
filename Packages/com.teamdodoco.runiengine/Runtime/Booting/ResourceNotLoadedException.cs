#nullable enable
using System;

namespace RuniEngine.Booting
{
    public class ResourceDataNotLoadedException : Exception
    {
        public static void Exception()
        {
            if (!BootLoader.allLoaded)
                throw new ResourceDataNotLoadedException();
        }

        public ResourceDataNotLoadedException() : base("리소스가 로드되지 않은 상태에서 작업을 진행할 수 없습니다")
        {

        }
    }
}
