#nullable enable
namespace RuniEngine.NBS
{
    public class NBSLayer
    {
        public string layerName { get; } = "";
        public byte layerLock { get; } = 0;
        public byte layerVolume { get; } = 1;
        public byte layerStereo { get; } = 0;

        public NBSLayer(string layerName, byte layerLock, byte layerVolume, byte layerStereo)
        {
            this.layerName = layerName;
            this.layerLock = layerLock;
            this.layerVolume = layerVolume;
            this.layerStereo = layerStereo;
        }

        public override string ToString() => $"(layername:{layerName}, layerLock:{layerLock}, layerVolume:{layerStereo}, layerStereo:{layerStereo})";
    }
}
