#nullable enable
namespace RuniEngine.NBS
{
    public readonly struct NBSCustomInstrument
    {
        public readonly string name;
        public readonly byte key;

        public NBSCustomInstrument(string name, byte key)
        {
            this.name = name;
            this.key = key;
        }

        public override string ToString() => $"(name:{name}, key:{key})";
    }
}
