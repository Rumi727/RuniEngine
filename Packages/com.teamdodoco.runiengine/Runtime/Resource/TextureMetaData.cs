#nullable enable
using UnityEngine;

namespace RuniEngine.Resource
{
    public class TextureMetaData
    {
        public FilterMode filterMode = FilterMode.Point;
        public bool generateMipmap = true;
        public TextureCompressionQuality compressionType = TextureCompressionQuality.none;
    }
}
