using RuniEngine.Resource;
using System;

namespace RuniEngine
{
    [Serializable]
    public struct NameSpacePathPair : IEquatable<NameSpacePathPair>
    {
        [FieldName("gui.path")] public string path;
        [FieldName("gui.nameSpace")] public string nameSpace;

        public NameSpacePathPair(string path)
        {
            nameSpace = "";
            this.path = path;
        }

        public NameSpacePathPair(string nameSpace, string path)
        {
            this.nameSpace = nameSpace;
            this.path = path;
        }

        public static implicit operator string(NameSpacePathPair value) => value.ToString();

        public static implicit operator NameSpacePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            return new NameSpacePathPair(nameSpace, value);
        }

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return path;
            else
                return nameSpace + ":" + path;
        }

        public static bool operator ==(NameSpacePathPair lhs, NameSpacePathPair rhs) => lhs.Equals(rhs);

        public static bool operator !=(NameSpacePathPair lhs, NameSpacePathPair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not NameSpacePathPair pair)
                return false;

            return nameSpace == pair.nameSpace && path == pair.path;
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 919747318;
                hash *= -569647612 + nameSpace.GetHashCode();
                hash *= -826125659 + path.GetHashCode();

                return hash;
            }
        }

        public readonly bool Equals(NameSpacePathPair other) => nameSpace == other.nameSpace && path == other.path;
    }
}
