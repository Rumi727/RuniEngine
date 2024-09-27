#nullable enable
using System;

namespace RuniEngine.Resource.Sounds
{
    public enum RawAudioLoadType
    {
        /// <summary>오디오를 즉시 로드합니다. (보통 효과음에서 사용)</summary>
        instant,
        /// <summary>오디오가 재생될 때, 백그라운드에서 로드합니다. (보통 이펙트가 필요한 배경 음악에서 사용)</summary>
        background,
        /// <summary>오디오를 스트리밍합니다. 메모리 사용량이 제일 적습니다. (보통 이펙트가 필요하지 않는 단순한 배경 음악에서 사용)</summary>
        stream
    }
}
