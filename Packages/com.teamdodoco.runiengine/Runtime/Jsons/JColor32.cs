using System;
using UnityEngine;

namespace RuniEngine.Jsons
{
    public struct JColor32 : IEquatable<JColor32>
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public static JColor32 zero { get; } = new JColor32();
        public static JColor32 one { get; } = new JColor32(1);

        public JColor32(Color value) : this((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round())
        {

        }

        public JColor32(JColor value) : this((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round())
        {

        }

        public JColor32(Color32 value) : this(value.r, value.g, value.b, value.a)
        {

        }

        public JColor32(byte value) => r = g = b = a = value;

        public JColor32(byte r, byte g, byte b) : this(r, g, b, 255)
        {

        }

        public JColor32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator JColor32(Vector3 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z);
        public static implicit operator JColor32(Vector4 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z, (byte)value.w);
        public static implicit operator JColor32(Rect value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.width, (byte)value.height);

        public static explicit operator JColor32(JVector3 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z);
        public static implicit operator JColor32(JVector4 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z, (byte)value.w);
        public static implicit operator JColor32(JRect value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.width, (byte)value.height);

        public static explicit operator Vector3(JColor32 value) => new Vector3(value.r, value.g, value.b);
        public static implicit operator Vector4(JColor32 value) => new Vector4(value.r, value.g, value.b, value.a);
        public static implicit operator Rect(JColor32 value) => new Rect(value.r, value.g, value.b, value.a);

        public static implicit operator JColor32(Color32 value) => new JColor32(value);
        public static implicit operator Color32(JColor32 value) => new Color32(value.r, value.g, value.b, value.a);

        public static implicit operator JColor32(Color value) => new JColor32(value);
        public static implicit operator Color(JColor32 value) => new Color(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f);


        public static implicit operator JColor32(JColor value) => new JColor32(value);
        public static implicit operator JColor(JColor32 value) => new JColor(value);

        public override readonly string ToString() => $"(r:{r}, g:{g}, b:{b}, a:{a})";
        public readonly bool Equals(JColor32 other) => r == other.r && g == other.g && b == other.b && a == other.a;
    }
}
