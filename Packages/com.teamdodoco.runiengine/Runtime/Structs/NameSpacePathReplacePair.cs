using RuniEngine.Resource;
using System;

namespace RuniEngine
{
    [Serializable]
    public struct NameSpacePathReplacePair : IEquatable<NameSpacePathReplacePair>
    {
        [FieldName("gui.path")] public string path;
        [FieldName("gui.nameSpace")] public string nameSpace;

        [FieldName("gui.replaces")] public ReplaceOldNewPair[] replaces;

        public NameSpacePathReplacePair(string path)
        {
            nameSpace = "";
            this.path = path;

            replaces = new ReplaceOldNewPair[0];
        }

        public NameSpacePathReplacePair(string nameSpace, string path, params ReplaceOldNewPair[] replaces)
        {
            this.nameSpace = nameSpace;
            this.path = path;

            this.replaces = replaces;
        }

        public static explicit operator string(NameSpacePathReplacePair value) => value.ToString();

        public static implicit operator NameSpacePathReplacePair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            return new NameSpacePathReplacePair(nameSpace, value);
        }

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return path;
            else
                return nameSpace + ":" + path;
        }

        public static bool operator ==(NameSpacePathReplacePair lhs, NameSpacePathReplacePair rhs) => lhs.Equals(rhs);

        public static bool operator !=(NameSpacePathReplacePair lhs, NameSpacePathReplacePair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not NameSpacePathReplacePair pair)
                return false;

            return nameSpace == pair.nameSpace && path == pair.path && replaces == pair.replaces;
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 859394412;
                hash *= -589877152 + nameSpace.GetHashCode();
                hash *= 268207203 + path.GetHashCode();
                hash *= 686815507 + replaces.GetHashCode();

                return hash;
            }
        }

        public readonly bool Equals(NameSpacePathReplacePair other) => nameSpace == other.nameSpace && path == other.path && replaces == other.replaces;
    }
}
