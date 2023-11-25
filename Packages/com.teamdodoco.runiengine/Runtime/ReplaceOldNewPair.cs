#nullable enable
using System;

namespace RuniEngine
{
    public struct ReplaceOldNewPair : IEquatable<ReplaceOldNewPair>
    {
        public string replaceOld;
        public string replaceNew;

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
