#nullable enable
using RuniEngine.Resource;
using System;
using System.IO;

namespace RuniEngine
{
    [Serializable]
    public struct NameSpaceIndexTypeNamePair : IEquatable<NameSpaceIndexTypeNamePair>
    {
        [FieldName("gui.nameSpace")] public string nameSpace;
        [FieldName("gui.type")] public string type;
        [FieldName("gui.name")] public string name;

        [FieldName("gui.index")] public int index;

        public NameSpaceIndexTypeNamePair(string type, string name)
        {
            nameSpace = "";
            index = 0;

            this.type = type;
            this.name = name;
        }

        public NameSpaceIndexTypeNamePair(string nameSpace, string type, string name)
        {
            this.nameSpace = nameSpace;
            index = 0;

            this.type = type;
            this.name = name;
        }

        public NameSpaceIndexTypeNamePair(string nameSpace, int index, string type, string name)
        {
            this.nameSpace = nameSpace;
            this.index = index;

            this.type = type;
            this.name = name;
        }

        public static implicit operator string(NameSpaceIndexTypeNamePair value) => value.ToString();

        public static implicit operator NameSpaceIndexTypeNamePair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);

            int spriteIndex = 0;
            if (value.Contains(':') && !int.TryParse(ResourceManager.GetNameSpace(value, out value), out spriteIndex))
                spriteIndex = -1;

            string type = ResourceManager.GetTextureType(value, out value);
            return new NameSpaceIndexTypeNamePair(nameSpace, spriteIndex, type, value);
        }

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return ResourceManager.defaultNameSpace + ":" + index + ":" + Path.Combine(type, name).UniformDirectorySeparatorCharacter();
            else
                return nameSpace + ":" + index + ":" + Path.Combine(type, name).UniformDirectorySeparatorCharacter();
        }

        public static bool operator ==(NameSpaceIndexTypeNamePair lhs, NameSpaceIndexTypeNamePair rhs) => lhs.Equals(rhs);

        public static bool operator !=(NameSpaceIndexTypeNamePair lhs, NameSpaceIndexTypeNamePair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not NameSpaceIndexTypeNamePair pair)
                return false;

            return nameSpace == pair.nameSpace && type == pair.type && name == pair.name && index == pair.index;
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = -212261093;
                hash *= -918515282 + nameSpace.GetHashCode();
                hash *= -975599599 + type.GetHashCode();
                hash *= -457296464 + name.GetHashCode();
                hash *= -500810711 + index.GetHashCode();

                return hash;
            }
        }

        public readonly bool Equals(NameSpaceIndexTypeNamePair other) => nameSpace == other.nameSpace && type == other.type && name == other.name && index == other.index;
    }
}
