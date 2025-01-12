#nullable enable
using Newtonsoft.Json;
using UnityEngine;

namespace RuniEngine.Resource.Images
{
    public struct TextureMetaData
    {
        public FilterMode filterMode
        {
            get => _filterMode ??= FilterMode.Point;
            set => _filterMode = value;
        }
        [JsonIgnore] FilterMode? _filterMode;

        public bool generateMipmap
        {
            get => _generateMipmap ??= true;
            set => _generateMipmap = value;
        }
        [JsonIgnore] bool? _generateMipmap;

        public TextureCompressionQuality compressionType
        {
            get => _compressionType ??= TextureCompressionQuality.none;
            set => _compressionType = value;
        }
        [JsonIgnore] TextureCompressionQuality? _compressionType;

        public TextureMetaData(FilterMode filterMode, bool generateMipmap, TextureCompressionQuality compressionType)
        {
            _filterMode = filterMode;
            _generateMipmap = generateMipmap;
            _compressionType = compressionType;
        }
    }
}
