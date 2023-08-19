#nullable enable
using RuniEngine.UI;
using UnityEngine;

namespace RuniEngine.Logo
{
    [ExecuteAlways, RequireComponent(typeof(CanvasGroup))]
    public class LogoAlpha : UIBase
    {
        public MainLogo? mainLogo => _mainLogo;
        [SerializeField, NotNullField] MainLogo? _mainLogo;

        public AnimationCurve animationCurve { get => _animationCurve; set => _animationCurve = value; }
        [SerializeField] AnimationCurve _animationCurve = new AnimationCurve();

        public CanvasGroup? canvasGroup => _canvasGroup = this.GetComponentFieldSave(_canvasGroup);
        CanvasGroup? _canvasGroup;

        void Update()
        {
            if (mainLogo == null || canvasGroup == null)
                return;

            canvasGroup.alpha = animationCurve.Evaluate(mainLogo.aniProgress);
        }
    }
}
