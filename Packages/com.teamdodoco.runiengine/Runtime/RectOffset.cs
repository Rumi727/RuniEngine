#nullable enable
using UnityEngine;

namespace RuniEngine
{
    public struct RectOffset
    {
        public RectOffset(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public Vector2 min;
        public Vector2 max;

        public static RectOffset zero => new RectOffset(Vector2.zero, Vector2.zero);
    }
}
