#nullable enable
using System;

namespace RuniEngine
{
    [Serializable]
    public struct VersionRange
    {
        public VersionRange(Version min, Version max)
        {
            this.min = min;
            this.max = max;
        }

        [FieldName("gui.min")] public Version min;
        [FieldName("gui.max")] public Version max;

        public readonly bool Contains(Version version) => min <= version && version <= max;
    }
}
