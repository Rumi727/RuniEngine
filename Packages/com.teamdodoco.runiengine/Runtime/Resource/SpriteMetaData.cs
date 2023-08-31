#nullable enable
using RuniEngine.Json;

namespace RuniEngine.Resource
{
    public class SpriteMetaData
    {
        public JRect rect = new JRect(float.MinValue);
        public JVector2 pivot = new JVector2(0.5f);
        public float pixelsPerUnit = 100;
        public JVector4 border = JVector4.zero;

        public void RectMinMax(float width, float height)
        {
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
        }

        public void PixelsPreUnitMinSet()
        {
            if (pixelsPerUnit < 0.01f)
                pixelsPerUnit = 0.01f;
        }
    }
}
