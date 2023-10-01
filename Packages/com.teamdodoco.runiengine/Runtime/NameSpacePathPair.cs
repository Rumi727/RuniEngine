#nullable enable
using RuniEngine.Resource;
using System;

namespace RuniEngine
{
    public struct NameSpacePathPair : IEquatable<NameSpacePathPair>
    {
        public string path;
        public string nameSpace;

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

        public override readonly int GetHashCode() => HashCode.Combine(nameSpace, path);

        public readonly bool Equals(NameSpacePathPair other) => nameSpace == other.nameSpace && path == other.path;
    }
}
