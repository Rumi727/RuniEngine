#nullable enable
using System;

namespace RuniEngine
{
    [Serializable]
    public struct ReplaceOldNewPair : IEquatable<ReplaceOldNewPair>
    {
        [FieldName("gui.replace.old")] public string replaceOld;
        [FieldName("gui.replace.new")] public string replaceNew;

        public ReplaceOldNewPair(string replaceOld, string replaceNew)
        {
            this.replaceOld = replaceOld;
            this.replaceNew = replaceNew;
        }

        public static bool operator ==(ReplaceOldNewPair lhs, ReplaceOldNewPair rhs) => lhs.Equals(rhs);

        public static bool operator !=(ReplaceOldNewPair lhs, ReplaceOldNewPair rhs) => !lhs.Equals(rhs);

        public override readonly bool Equals(object? obj)
        {
            if (obj is not ReplaceOldNewPair pair)
                return false;

            return replaceOld == pair.replaceOld && replaceNew == pair.replaceNew;
        }

        public override readonly int GetHashCode() => HashCode.Combine(replaceOld, replaceNew);

        public readonly bool Equals(ReplaceOldNewPair other) => replaceOld == other.replaceOld && replaceNew == other.replaceNew;
    }
}
