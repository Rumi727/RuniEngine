#nullable enable
using RuniEngine.Resource.Sounds;
using System;

namespace RuniEngine.Sounds
{
    public static class AudioUtility
    {
        /// <summary>
        /// 가능하면 루프를 적용시킨 샘플의 인덱스의 값을 가져옵니다
        /// </summary>
        /// <param name="loopedSamples">결과물을 반환하는 배열입니다<br/>배열의 길이는 오디오 채널과 같아야합니다 (안맞는 경우, 자동으로 처리)</param>
        /// <param name="index">인덱스</param>
        /// <param name="samples">가져올 샘플</param>
        /// <param name="loop">루프 여부</param>
        /// <param name="loopStartIndex">루프를 시작할 인덱스</param>
        /// <param name="loopOffsetIndex">루프 오프셋</param>
        public static void GetLoopedSamples(ref float[] loopedSamples, long index, RawAudioClip samples, bool loop, long loopStartIndex, long loopOffsetIndex)
        {
            int audioChannels = samples.channels;
            if (loopedSamples.Length != audioChannels)
                loopedSamples = new float[audioChannels];

            long sampleIndex = index;
            long sampleCount = samples.arrayLength;

            if (loop)
                sampleIndex %= sampleCount - 1;

            if (sampleIndex >= 0 && sampleIndex < sampleCount)
            {
                for (int i = 0; i < audioChannels; i++)
                    loopedSamples[i] = samples[sampleIndex + i];

                //루프
                if (loop && !samples.loader.isStream)
                {
                    long rawLoopOffsetSampleIndex = sampleIndex - (sampleCount - loopOffsetIndex);
                    long loopOffsetSampleIndex = loopStartIndex + (rawLoopOffsetSampleIndex % (sampleCount - loopStartIndex));

                    if (rawLoopOffsetSampleIndex >= 0 && loopOffsetSampleIndex >= 0 && loopOffsetSampleIndex < sampleCount)
                    {
                        for (int i = 0; i < audioChannels; i++)
                            loopedSamples[i] += samples[loopOffsetSampleIndex + i];
                    }
                }
            }
            else
                Array.Fill(loopedSamples, 0);
        }

        /// <summary>
        /// 모노 &lt;-&gt; 스테레오 또는 서로 같은 채널인 경우만 지원합니다
        /// </summary>
        /// <param name="mixedSamples">채널을 합칠 대상입니다<br/>배열의 길이는 시스템 채널과 같아야합니다</param>
        /// <param name="loopedSamples">채널을 합칠 대상입니다<br/>배열의 길이는 오디오 채널과 같아야합니다</param>
        /// <param name="volume">볼륨</param>
        public static void MixChannel(ref float[] mixedSamples, float[] loopedSamples)
        {
            //Array.Fill(mixedSamples, 0);

            int systemChannels = mixedSamples.Length.Clamp(0, 2);
            int audioChannels = loopedSamples.Length.Clamp(0, 2);

            if (systemChannels == 0 || audioChannels == 0)
            {
                Array.Fill(mixedSamples, 0);
                return;
            }

            if (systemChannels == audioChannels)
            {
                Array.Copy(loopedSamples, mixedSamples, systemChannels);
                return;
            }

            if (systemChannels < audioChannels)
            {
                mixedSamples[0] = loopedSamples[0] + loopedSamples[1] * 0.5f;

                /*for (int i = 0; i < audioChannels; i += systemChannels)
                {
                    for (int j = 0; j < systemChannels; j++)
                        mixedSamples[j] += loopedSamples[i + j] * ((float)systemChannels / audioChannels) * volume;
                }*/
            }
            else if (systemChannels > audioChannels)
            {
                mixedSamples[0] = loopedSamples[0] * 0.6666666667f;
                mixedSamples[1] = loopedSamples[0] * 0.6666666667f;

                /*for (int i = 0; i < systemChannels; i += audioChannels)
                {
                    for (int j = 0; j < audioChannels; j++)
                        mixedSamples[i + j] += loopedSamples[j] * volume;
                }*/
            }
        }

        /// <summary>샘플 배열에 스테레오 값을 적용합니다</summary>
        public static void SetPanStereo(ref float[] mixedSamples, float panStereo, bool spatial, float spatialStereo)
        {
            if (mixedSamples.Length < 2)
                return;

            float left = mixedSamples[0];
            float right = mixedSamples[1];

            float stereo;
            if (spatial)
                stereo = (float)panStereo.Lerp(spatialStereo, spatialStereo.Abs());
            else
                stereo = (float)panStereo;

            float leftStereo = (-stereo).Clamp01();
            float rightStereo = stereo.Clamp01();

            mixedSamples[0] = (left + 0f.LerpUnclamped(right, leftStereo)) * (1 - rightStereo) * 1f.Lerp(0.5f, stereo.Abs());
            mixedSamples[1] = (right + 0f.LerpUnclamped(left, rightStereo)) * (1 - leftStereo) * 1f.Lerp(0.5f, stereo.Abs());
        }
    }
}
