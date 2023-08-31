#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.Json
{
    public struct JVector2 : IEquatable<JVector2>
    {
        public float x;
        public float y;

        public static JVector2 zero { get; } = new JVector2();
        public static JVector2 one { get; } = new JVector2(1);

        public JVector2(Vector2 value) : this(value.x, value.y)
        {

        }

        public JVector2(float value) => x = y = value;
        public JVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator JVector3(JVector2 value) => new JVector3(value.x, value.y);
        public static implicit operator JVector4(JVector2 value) => new JVector4(value.x, value.y);

        public static implicit operator JVector2(JRect value) => new JVector2(value.x, value.y);

        public static implicit operator JVector2(Vector2 value) => new JVector2(value);
        public static implicit operator JVector2(Vector3 value) => new JVector2(value);
        public static implicit operator JVector2(Vector4 value) => new JVector2(value);
        public static implicit operator Vector2(JVector2 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector3(JVector2 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector4(JVector2 value) => new Vector4(value.x, value.y);

        public override readonly string ToString() => $"(x: {x}, y: {y})";
        public readonly bool Equals(JVector2 other) => x == other.x && y == other.y;
    }
}
