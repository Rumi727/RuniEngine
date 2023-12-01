#nullable enable
using System;
using UnityEngine;

namespace RuniEngine
{
    [Serializable]
    public struct RectCorner
    {
        public Rect rect
        {
            readonly get => this;
            set
            {
                Rect rect = value;

                bottomLeft = new Vector2(rect.xMin, rect.yMin);
                topLeft = new Vector2(rect.xMin, rect.yMax);
                topRight = new Vector2(rect.xMax, rect.yMax);
                bottomRight = new Vector2(rect.xMax, rect.yMin);
            }
        }

        [FieldName("gui.bottom_left")] public Vector2 bottomLeft;
        [FieldName("gui.top_left")] public Vector2 topLeft;
        [FieldName("gui.top_right")] public Vector2 topRight;
        [FieldName("gui.bottom_right")] public Vector2 bottomRight;



        public static implicit operator Rect(RectCorner value) => new Rect(value.bottomLeft, value.topRight - value.bottomLeft);
        public static implicit operator RectCorner(Rect value) => new RectCorner(value);



        public RectCorner(Vector2 bottomLeft, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight)
        {
            this.bottomLeft = bottomLeft;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
        }

        public RectCorner(Rect rect)
        {
            bottomLeft = new Vector2(rect.xMin, rect.yMin);
            topLeft = new Vector2(rect.xMin, rect.yMax);
            topRight = new Vector2(rect.xMax, rect.yMax);
            bottomRight = new Vector2(rect.xMax, rect.yMin);
        }

        public RectCorner(Vector2 min, Vector2 max)
        {
            bottomLeft = new Vector2(min.x, min.y);
            topLeft = new Vector2(min.x, max.y);
            topRight = new Vector2(max.x, max.y);
            bottomRight = new Vector2(max.x, min.y);
        }
    }
}
