#nullable enable
using RuniEngine.Resource;
using System;
using System.IO;

namespace RuniEngine
{
    public struct NameSpaceIndexTypePathPair : IEquatable<NameSpaceIndexTypePathPair>
    {
        public string type;
        public string path;
        public string nameSpace;

        public int index;

        public NameSpaceIndexTypePathPair(string type, string path)
        {
            nameSpace = "";
            index = 0;

            this.type = type;
            this.path = path;
        }

        public NameSpaceIndexTypePathPair(string nameSpace, string type, string path)
        {
            this.nameSpace = nameSpace;
            index = 0;

            this.type = type;
            this.path = path;
        }

        public NameSpaceIndexTypePathPair(string nameSpace, int index, string type, string path)
        {
            this.nameSpace = nameSpace;
            this.index = index;

            this.type = type;
            this.path = path;
        }

        public static implicit operator string(NameSpaceIndexTypePathPair value) => value.ToString();

        public static implicit operator NameSpaceIndexTypePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);

            int spriteIndex = 0;
            if (value.Contains(':') && !int.TryParse(ResourceManager.GetNameSpace(value, out value), out spriteIndex))
                spriteIndex = -1;

            string type = ResourceManager.GetTextureType(value, out value);
            return new NameSpaceIndexTypePathPair(nameSpace, spriteIndex, type, value);
        }

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return ResourceManager.defaultNameSpace + ":" + index + ":" + Path.Combine(type, path).UniformDirectorySeparatorCharacter();
            else
                return nameSpace + ":" + index + ":" + Path.Combine(type, path).UniformDirectorySeparatorCharacter();
        }

        public static bool operator ==(NameSpaceIndexTypePathPair lhs, NameSpaceIndexTypePathPair rhs) => lhs.Equals(rhs);

        public static bool operator !=(NameSpaceIndexTypePathPair lhs, NameSpaceIndexTypePathPair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not NameSpaceIndexTypePathPair pair)
                return false;

            return nameSpace == pair.nameSpace && type == pair.type && path == pair.path && index == pair.index;
        }

        public override readonly int GetHashCode() => HashCode.Combine(nameSpace, type, path, index);

        public readonly bool Equals(NameSpaceIndexTypePathPair other) => nameSpace == other.nameSpace && type == other.type && path == other.path && index == other.index;
    }
}
