#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.UI.Themes
{
    public struct LayoutGroupStyle : IEquatable<LayoutGroupStyle>
    {
        public LayoutGroupStyle(RectOffset padding, float spacing, TextAnchor childAlignment, bool reverseArrangement)
        {
            this.padding = padding;
            this.spacing = spacing;

            this.childAlignment = childAlignment;
            this.reverseArrangement = reverseArrangement;
        }

        public RectOffset padding;
        public float spacing;

        public TextAnchor childAlignment;
        public bool reverseArrangement;

        public override readonly bool Equals(object? obj) => obj is LayoutGroupStyle info && Equals(info);

        public readonly bool Equals(LayoutGroupStyle other) =>
            childAlignment == other.childAlignment &&
            reverseArrangement == other.reverseArrangement;

        public override readonly int GetHashCode() => HashCode.Combine(childAlignment, reverseArrangement);

        public static bool operator ==(LayoutGroupStyle left, LayoutGroupStyle right) => left.Equals(right);
        public static bool operator !=(LayoutGroupStyle left, LayoutGroupStyle right) => !left.Equals(right);
    }
}
