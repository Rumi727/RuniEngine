#nullable enable
using System;

namespace RuniEngine.Booting
{
    public class BasicDataNotLoadedException : Exception
    {
        public BasicDataNotLoadedException() : base("기초 데이터가 로드되지 않은 상태에서 작업을 진행할 수 없습니다")
        {

        }
    }
}
