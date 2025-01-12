#nullable enable
namespace RuniEngine.Resource
{
    public sealed class StreamingIOHandler : FileIOHandler
    {
        public static StreamingIOHandler instance { get; } = new StreamingIOHandler();

        StreamingIOHandler() : base(Kernel.streamingAssetsPath) { }
    }
}
