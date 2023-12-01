#nullable enable
using System;
using UnityEngine;

namespace RuniEngine
{
    [Serializable]
    public struct RectOffset
    {
        public RectOffset(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        [FieldName("gui.min")] public Vector2 min;
        [FieldName("gui.max")] public Vector2 max;

        public static RectOffset zero => new RectOffset(Vector2.zero, Vector2.zero);
    }
}
