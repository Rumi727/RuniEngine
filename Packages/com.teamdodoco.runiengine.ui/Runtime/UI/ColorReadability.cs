#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Graphic))]
    public sealed class ColorReadability : UIAniBase
    {
        public CanvasRenderer? targetCanvasRenderer => _targetCanvasRenderer;
        [SerializeField, NotNullField] CanvasRenderer? _targetCanvasRenderer;

        public Graphic? targetGraphic => _targetGraphic;
        [SerializeField, NotNullField] Graphic? _targetGraphic;

        public Color whiteColor { get => _whiteColor; set => _whiteColor = value; }
        [SerializeField] Color _whiteColor = Color.white;

        public Color blackColor { get => _blackColor; set => _blackColor = value; }
        [SerializeField] Color _blackColor = Color.black;

        Color color = Color.white;
        public override void LayoutUpdate()
        {
            if (targetCanvasRenderer == null || targetGraphic == null || graphic == null || targetCanvasRenderer == graphic)
                return;

            color = GetReadbilityColor(targetGraphic.color * targetCanvasRenderer.GetColor(), whiteColor, blackColor);
            graphic.color = graphic.color.Lerp(color, currentLerpSpeed);
        }

        public static Color GetReadbilityColor(float color, Color white, Color black)
        {
            if (color <= 0.5f)
                return white;
            else
                return black;
        }

        public static Color GetReadbilityColor(Color color, Color white, Color black)
        {
            float average = (color.r + color.g + color.b) / 3;

            if (average <= 0.5f)
                return white;
            else
                return black;
        }
    }
}