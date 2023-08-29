#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Fitter
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TargetRectTransformFitter : FitterAniBase
    {
        public RectTransform? targetRectTransform { get => _targetRectTransform; set => _targetRectTransform = value; }
        [SerializeField, NotNullField] RectTransform? _targetRectTransform;

        

        protected override void OnEnable()
        {
            base.OnEnable();
            Canvas.preWillRenderCanvases += SetDirty;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Canvas.preWillRenderCanvases -= SetDirty;
        }

        readonly Vector3[] fourCornersArray = new Vector3[4];
        RectCorner cachedRectCorner;
        public override void SetDirty()
        {
            if (targetRectTransform == null)
                return;

            targetRectTransform.GetWorldCorners(fourCornersArray);
            cachedRectCorner = new RectCorner(fourCornersArray[0], fourCornersArray[1], fourCornersArray[2], fourCornersArray[3]);

            base.SetDirty();
        }

        public override void LayoutUpdate()
        {
            if (targetRectTransform == null)
                return;

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, rectTransform, DrivenTransformProperties.All);
            }

            worldCorners = worldCorners.Lerp(cachedRectCorner, currentLerpSpeed);

            rectTransform.position = rectTransform.position.Lerp(targetRectTransform.position, currentLerpSpeed);
            rectTransform.localScale = rectTransform.localScale.Lerp(targetRectTransform.localScale, currentLerpSpeed);
            rectTransform.rotation = rectTransform.rotation.Lerp(targetRectTransform.rotation, currentLerpSpeed);

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
