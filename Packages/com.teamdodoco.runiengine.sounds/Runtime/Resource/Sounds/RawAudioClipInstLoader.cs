#nullable enable
using NAudio.Wave;

namespace RuniEngine.Resource.Sounds
{
    public sealed class RawAudioClipInstLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipInstLoader(WaveStream stream) : base(stream)
        {
            ISampleProvider reader = stream.ToSampleProvider();

            datas = new float[arrayLength];
            float[] buffer = new float[frequency * channels]; //1초 버퍼

            long position = 0;
            int readSampleLength;

            long datasLength = datas.LongLength;
            int bufferLength = buffer.Length;

            while ((readSampleLength = reader.Read(buffer, 0, bufferLength)) > 0)
            {
                for (int i = 0; i < readSampleLength; i++)
                {
                    if (position + i >= datasLength)
                        break;

                    datas[position + i] = buffer[i];
                }

                position += readSampleLength;
            }

            stream.Dispose();
        }

        readonly float[] datas;

        /// <summary>Thread-safe</summary>
        public override bool isLoading => false;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => true;

        /// <summary>Thread-safe</summary>
        public override bool isStream => false;

        /// <summary>Thread-safe</summary>
        public override float GetSample(long index) => datas[index];
    }
}
