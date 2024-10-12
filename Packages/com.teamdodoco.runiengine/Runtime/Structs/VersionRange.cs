using System;

namespace RuniEngine
{
    [Serializable]
    public struct VersionRange : IEquatable<Version>, IEquatable<VersionRange>
    {
        public VersionRange(Version version)
        {
            min = version;
            max = version;
        }

        public VersionRange(Version min, Version max)
        {
            this.min = min;
            this.max = max;
        }

        [FieldName("gui.min")] public Version min;
        [FieldName("gui.max")] public Version max;

        public static bool operator ==(VersionRange lhs, VersionRange rhs) => lhs.min == rhs.min && lhs.max == rhs.max;
        public static bool operator !=(VersionRange lhs, VersionRange rhs) => !(lhs == rhs);

        public static bool operator ==(VersionRange lhs, Version rhs) => lhs.min == rhs && lhs.max == rhs;
        public static bool operator !=(VersionRange lhs, Version rhs) => !(lhs == rhs);

        public readonly bool Contains(Version version) => version >= min && version <= max;
        public readonly bool Contains(VersionRange range) => range.min >= min && range.max <= max;

        public readonly bool Equals(Version other) => min == other && max == other;
        public readonly bool Equals(VersionRange other) => this == other;

        public override readonly bool Equals(object obj)
        {
            if (obj is VersionRange range)
                return Equals(range);
            else if (obj is Version version)
                return Equals(version);

            return false;
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 92381513;
                hash *= 582934 + min.GetHashCode();
                hash *= 3829571 + max.GetHashCode();

                return hash;
            }
        }
    }
}
