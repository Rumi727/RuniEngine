using System;
using UnityEngine;

namespace RuniEngine.Jsons
{
    public struct JRect : IEquatable<JRect>
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public static JRect zero { get; } = new JRect();
        public static JRect one { get; } = new JRect(1);

        public JRect(Rect value) : this(value.x, value.y, value.width, value.height)
        {

        }

        public JRect(float value) => x = y = width = height = value;

        public JRect(float x, float y) : this(x, y, 0, 0)
        {

        }

        public JRect(float x, float y, float width) : this(x, y, width, 0)
        {

        }

        public JRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static explicit operator JRect(JVector2 value) => new JRect(value.x, value.y);
        public static explicit operator JRect(JVector3 value) => new JRect(value.x, value.y, value.z);
        public static implicit operator JRect(JVector4 value) => new JRect(value.x, value.y, value.z, value.w);
        public static implicit operator JRect(JColor value) => new JRect(value.r, value.g, value.b, value.a);

        public static explicit operator JRect(Vector2 value) => new JRect(value.x, value.y);
        public static explicit operator JRect(Vector3 value) => new JRect(value.x, value.y, value.z);
        public static implicit operator JRect(Vector4 value) => new JRect(value.x, value.y, value.z, value.w);
        public static implicit operator JRect(Color value) => new JRect(value.r, value.g, value.b, value.a);

        public static explicit operator Vector2(JRect value) => new Vector2(value.x, value.y);
        public static explicit operator Vector3(JRect value) => new Vector3(value.x, value.y, value.width);
        public static implicit operator Vector4(JRect value) => new Vector4(value.x, value.y, value.width, value.height);
        public static implicit operator Color(JRect value) => new Color(value.x, value.y, value.width, value.height);

        public static implicit operator JRect(Rect value) => new JRect(value);
        public static implicit operator Rect(JRect value) => new Rect() { x = value.x, y = value.y, width = value.width, height = value.height };

        public override readonly string ToString() => $"(x:{x}, y:{y}, width:{width}, height:{height})";
        public readonly bool Equals(JRect other) => x == other.x && y == other.y && width == other.width && height == other.height;
    }
}
