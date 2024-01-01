#nullable enable
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace RuniEngine
{
    [Serializable]
    public struct RectOffset : IEquatable<RectOffset>
    {
        public RectOffset(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public RectOffset(Vector2Int min, Vector2Int max)
        {
            left = min.x;
            right = max.x;
            top = max.y;
            bottom = min.y;
        }

        [FieldName("gui.left")] public int left;
        [FieldName("gui.right")] public int right;
        [FieldName("gui.top")] public int top;
        [FieldName("gui.bottom")] public int bottom;

        [JsonIgnore]
        public Vector2Int min
        {
            readonly get => new Vector2Int(left, bottom);
            set
            {
                left = value.x;
                bottom = value.y;
            }
        }

        [JsonIgnore]
        public Vector2Int max
        {
            readonly get => new Vector2Int(right, top);
            set
            {
                right = value.x;
                top = value.y;
            }
        }

        [JsonIgnore] public static RectOffset zero => new RectOffset(Vector2Int.zero, Vector2Int.zero);



        public static bool operator ==(RectOffset left, RectOffset right) => left.Equals(right);
        public static bool operator !=(RectOffset left, RectOffset right) => !(left == right);



        public static implicit operator RectOffset(UnityEngine.RectOffset v)
        {
            RectOffset result = new RectOffset
            {
                left = v.left,
                right = v.right,
                top = v.top,
                bottom = v.bottom
            };

            return result;
        }

        public static implicit operator UnityEngine.RectOffset(RectOffset v)
        {
            UnityEngine.RectOffset result = new UnityEngine.RectOffset
            {
                left = (int)v.left,
                right = (int)v.right,
                top = (int)v.top,
                bottom = (int)v.bottom
            };

            return result;
        }



        public readonly bool Equals(RectOffset other) => left.Equals(other.left) && right.Equals(other.right) && top.Equals(other.top) && bottom.Equals(other.bottom);
        public override readonly bool Equals(object? obj) => obj is RectOffset result && Equals(result);

        public override readonly int GetHashCode() => HashCode.Combine(left, right, top, bottom);
    }
}
