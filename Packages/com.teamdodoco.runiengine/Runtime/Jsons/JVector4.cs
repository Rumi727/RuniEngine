using System;
using UnityEngine;

namespace RuniEngine.Jsons
{
    public struct JVector4 : IEquatable<JVector4>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static JVector4 zero { get; } = new JVector4();
        public static JVector4 one { get; } = new JVector4(1);

        public JVector4(Vector4 value) : this(value.x, value.y, value.z, value.w)
        {

        }

        public JVector4(float value) => x = y = z = w = value;

        public JVector4(float x, float y) : this(x, y, 0, 0)
        {

        }

        public JVector4(float x, float y, float z) : this(x, y, z, 0)
        {

        }

        public JVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static explicit operator JVector2(JVector4 value) => new JVector2(value.x, value.y);
        public static explicit operator JVector3(JVector4 value) => new JVector3(value.x, value.y, value.z);

        public static implicit operator JVector4(JRect value) => new JVector4(value.x, value.y, value.width, value.height);
        public static implicit operator JVector4(Rect value) => new JVector4(value.x, value.y, value.width, value.height);

        public static implicit operator JVector4(Vector2 value) => new JVector4(value);
        public static implicit operator JVector4(Vector3 value) => new JVector4(value);
        public static implicit operator JVector4(Vector4 value) => new JVector4(value);
        public static explicit operator Vector2(JVector4 value) => new Vector2(value.x, value.y);
        public static explicit operator Vector3(JVector4 value) => new Vector3(value.x, value.y, value.z);
        public static implicit operator Vector4(JVector4 value) => new Vector4(value.x, value.y, value.z, value.w);

        public override readonly string ToString() => $"({x}, {y}, {z}, {w})";
        public readonly bool Equals(JVector4 other) => x == other.x && y == other.y && z == other.z && w == other.w;
    }
}
