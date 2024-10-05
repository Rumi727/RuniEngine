#nullable enable
using System;

namespace RuniEngine.Rhythms
{
    public interface IBeatValuePair : IComparable, IComparable<IBeatValuePair>, IComparable<double>
    {
        /// <summary>
        /// value의 타입
        /// <para></para>
        /// 리플랙션에서도 value에서 타입을 가져올 수 있지만 value가 null인 경우 가져오지 못하기 때문에 따로 지정함
        /// </summary>
        Type type { get; }

        double beat { get; set; }
        object? value { get; set; }

        //bool confused { get; set; }
    }
}
