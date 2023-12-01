#nullable enable
using System;
using UnityEngine;

namespace RuniEngine
{
    [Serializable]
    public struct RectOffset : IEquatable<RectOffset>
    {
        public RectOffset(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        [FieldName("gui.min")] public Vector2 min;
        [FieldName("gui.max")] public Vector2 max;

        public static RectOffset zero => new RectOffset(Vector2.zero, Vector2.zero);



        public static bool operator ==(RectOffset left, RectOffset right) => left.Equals(right);
        public static bool operator !=(RectOffset left, RectOffset right) => !(left == right);



        public readonly bool Equals(RectOffset other) => min.Equals(other.min) && max.Equals(other.max);
        public override readonly bool Equals(object? obj) => obj is RectOffset result && Equals(result);

        public override readonly int GetHashCode() => HashCode.Combine(min, max);
    }
}
