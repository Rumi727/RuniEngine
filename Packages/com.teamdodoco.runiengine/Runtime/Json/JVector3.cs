#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.Json
{
    public struct JVector3 : IEquatable<JVector3>
    {
        public float x;
        public float y;
        public float z;

        public static JVector3 zero { get; } = new JVector3();
        public static JVector3 one { get; } = new JVector3(1);

        public JVector3(Vector3 value) : this(value.x, value.y, value.z)
        {

        }

        public JVector3(float value) => x = y = z = value;

        public JVector3(float x, float y) : this(x, y, 0)
        {

        }

        public JVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static explicit operator JVector2(JVector3 value) => new JVector2(value.x, value.y);
        public static implicit operator JVector4(JVector3 value) => new JVector4(value.x, value.y, value.z);

        public static implicit operator JVector3(JRect value) => new JVector3(value.x, value.y, value.width);

        public static implicit operator JVector3(Vector2 value) => new JVector3(value);
        public static implicit operator JVector3(Vector3 value) => new JVector3(value);
        public static implicit operator JVector3(Vector4 value) => new JVector3(value);
        public static explicit operator Vector2(JVector3 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector3(JVector3 value) => new Vector3(value.x, value.y, value.z);
        public static implicit operator Vector4(JVector3 value) => new Vector4(value.x, value.y, value.z);

        public override readonly string ToString() => $"({x}, {y}, {z})";
        public readonly bool Equals(JVector3 other) => x == other.x && y == other.y && z == other.z;
    }
}
