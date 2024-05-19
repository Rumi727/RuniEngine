#nullable enable
namespace RuniEngine
{
    public struct VersionRange
    {
        public VersionRange(Version min, Version max)
        {
            this.min = min;
            this.max = max;
        }

        public Version min;
        public Version max;

        public readonly bool Contains(Version version) => min <= version && version <= max;
    }
}
