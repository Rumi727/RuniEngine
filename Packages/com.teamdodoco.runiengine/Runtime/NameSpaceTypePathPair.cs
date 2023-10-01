#nullable enable
using RuniEngine.Resource;
using System;
using System.IO;

namespace RuniEngine
{
    public struct NameSpaceTypePathPair : IEquatable<NameSpaceTypePathPair>
    {
        public string type;
        public string path;
        public string nameSpace;

        public NameSpaceTypePathPair(string type, string path)
        {
            nameSpace = "";

            this.type = type;
            this.path = path;
        }

        public NameSpaceTypePathPair(string nameSpace, string type, string path)
        {
            this.nameSpace = nameSpace;

            this.type = type;
            this.path = path;
        }

        public static implicit operator string(NameSpaceTypePathPair value) => value.ToString();

        public static implicit operator NameSpaceTypePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            string type = ResourceManager.GetTextureType(value, out value);

            return new NameSpaceTypePathPair(nameSpace, type, value);
        }

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return Path.Combine(type, path);
            else
                return nameSpace + ":" + Path.Combine(type, path).UniformDirectorySeparatorCharacter();
        }

        public static bool operator ==(NameSpaceTypePathPair lhs, NameSpaceTypePathPair rhs) => lhs.Equals(rhs);

        public static bool operator !=(NameSpaceTypePathPair lhs, NameSpaceTypePathPair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not NameSpaceTypePathPair pair)
                return false;

            return nameSpace == pair.nameSpace && type == pair.type && path == pair.path;
        }

        public override readonly int GetHashCode() => HashCode.Combine(nameSpace, type, path);

        public readonly bool Equals(NameSpaceTypePathPair other) => nameSpace == other.nameSpace && type == other.type && path == other.path;
    }
}
