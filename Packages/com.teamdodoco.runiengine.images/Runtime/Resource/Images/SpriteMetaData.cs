using Newtonsoft.Json;
using RuniEngine.Jsons;

namespace RuniEngine.Resource.Images
{
    public struct SpriteMetaData
    {
        public JRect rect
        {
            get => _rect ??= new JRect(float.MinValue);
            set => _rect = value;
        }
        [JsonIgnore] JRect? _rect;

        public JVector2 pivot
        {
            get => _pivot ??= new JVector2(0.5f);
            set => _pivot = value;
        }
        [JsonIgnore] JVector2? _pivot;

        public float pixelsPerUnit
        {
            get
            {
                _pixelsPerUnit ??= 100;
                _pixelsPerUnit = _pixelsPerUnit.Value.Clamp(0.01f);

                return _pixelsPerUnit.Value;
            }
            set => _pixelsPerUnit = value;
        }
        [JsonIgnore] float? _pixelsPerUnit;

        public JVector4 border
        {
            get => _border ??= JVector4.zero;
            set => _border = value;
        }
        [JsonIgnore] JVector4? _border;

        public void RectMinMax(float width, float height)
        {
            JRect rect = this.rect;
            if (rect.x <= float.MinValue)
            {
                rect.x = 0;
                rect.y = 0;
                rect.width = width;
                rect.height = height;
            }

            if (rect.width < 1)
                rect.width = 1;
            if (rect.width > width)
                rect.width = width;
            if (rect.height < 1)
                rect.height = 1;
            if (rect.height > height)
                rect.height = height;

            if (rect.x < 0)
                rect.x = 0;
            if (rect.x > width - rect.width)
                rect.x = width - rect.width;
            if (rect.y < 0)
                rect.y = 0;
            if (rect.y > height - rect.height)
                rect.y = height - rect.height;

            this.rect = rect;
        }
    }
}
