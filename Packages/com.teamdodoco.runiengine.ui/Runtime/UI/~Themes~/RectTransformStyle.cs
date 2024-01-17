#nullable enable
using RuniEngine.Json;

namespace RuniEngine.UI.Themes
{
    public struct RectTransformStyle
    {
        public RectOffset offset;
        
        public JVector2 anchorMin;
        public JVector2 anchorMax;

        public JVector2 pivot;

        public RectTransformStyle(RectOffset offset, JVector2 anchorMin, JVector2 anchorMax, JVector2 pivot)
        {
            this.offset = offset;

            this.anchorMin = anchorMin;
            this.anchorMax = anchorMax;

            this.pivot = pivot;
        }
    }
}
