#nullable enable
using UnityEngine;

namespace RuniEngine.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class AutoColorAnimation : UIAniBase
    {
        public bool useCustomColor { get => _useCustomColor; set => _useCustomColor = value; }
        [SerializeField] bool _useCustomColor = false;

        public Color color { get => _color; set => _color = value; }
        [SerializeField] Color _color = Color.white;

        public Color offset { get => _offset; set => _offset = value; }
        [SerializeField] Color _offset = Color.white;

        Color cachedColor;
        public override void SetDirty()
        {
            if (useCustomColor)
                cachedColor = color;
            else
                cachedColor = UIManager.UserData.mainColor * offset;
        }

        public override void LayoutUpdate()
        {
            if (graphic == null)
                return;

            graphic.color = graphic.color.Lerp(cachedColor, currentLerpSpeed);
        }
    }
}
