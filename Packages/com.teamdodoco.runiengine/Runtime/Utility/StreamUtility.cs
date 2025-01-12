#nullable enable
using System.IO;

namespace RuniEngine
{
    public static class StreamUtility
    {
        public static byte[] ReadFully(this Stream stream)
        {
            using MemoryStream result = new MemoryStream();

            stream.CopyTo(result);
            return result.ToArray();
        }
    }
}
