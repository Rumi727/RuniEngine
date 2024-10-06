using System;
using UnityEngine;

namespace RuniEngine.Jsons
{
    public struct JColor : IEquatable<JColor>
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public static JColor zero { get; } = new JColor();
        public static JColor one { get; } = new JColor(1);

        public JColor(Color value) : this(value.r, value.g, value.b, value.a)
        {

        }

        public JColor(Color32 value) : this(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f)
        {

        }

        public JColor(JColor32 value) : this(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f)
        {

        }

        public JColor(float value) => r = g = b = a = value;

        public JColor(float r, float g, float b) : this(r, g, b, 1)
        {

        }

        public JColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator JColor(Vector3 value) => new JColor(value.x, value.y, value.z);
        public static implicit operator JColor(Vector4 value) => new JColor(value.x, value.y, value.z, value.w);
        public static implicit operator JColor(Rect value) => new JColor(value.x, value.y, value.width, value.height);

        public static explicit operator JColor(JVector3 value) => new JColor(value.x, value.y, value.z);
        public static implicit operator JColor(JVector4 value) => new JColor(value.x, value.y, value.z, value.w);
        public static implicit operator JColor(JRect value) => new JColor(value.x, value.y, value.width, value.height);

        public static explicit operator Vector3(JColor value) => new Vector3(value.r, value.g, value.b);
        public static implicit operator Vector4(JColor value) => new Vector4(value.r, value.g, value.b, value.a);
        public static implicit operator Rect(JColor value) => new Rect(value.r, value.g, value.b, value.a);

        public static implicit operator JColor(Color32 value) => new JColor(value);
        public static implicit operator Color32(JColor value) => new Color32((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round());

        public static implicit operator JColor(Color value) => new JColor(value);
        public static implicit operator Color(JColor value) => new Color(value.r, value.g, value.b, value.a);

        public override readonly string ToString() => $"(r:{r}, g:{g}, b:{b}, a:{a})";
        public readonly bool Equals(JColor other) => r == other.r && g == other.g && b == other.b && a == other.a;
    }
}
