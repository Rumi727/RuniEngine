#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Fitter
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TargetRectTransformFitter : FitterBase
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
        bool isRefresh = false;
        public override void SetDirty()
        {
            if (isRefresh || targetRectTransform == null)
                return;

            try
            {
                isRefresh = true;

                if (!Kernel.isPlaying)
                {
                    tracker.Clear();
                    tracker.Add(this, rectTransform, DrivenTransformProperties.All);
                }

                targetRectTransform.GetWorldCorners(fourCornersArray);
                worldCorners = new RectCorner(fourCornersArray[0], fourCornersArray[1], fourCornersArray[2], fourCornersArray[3]);

                rectTransform.position = targetRectTransform.position;
                rectTransform.localScale = targetRectTransform.localScale;
                rectTransform.rotation = targetRectTransform.rotation;

                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
            }
            finally
            {
                isRefresh = false;
            }
        }
    }
}
