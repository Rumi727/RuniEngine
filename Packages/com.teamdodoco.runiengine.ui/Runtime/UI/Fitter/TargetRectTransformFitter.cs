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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Canvas.preWillRenderCanvases -= SetDirty;
        }

        readonly Vector3[] fourCornersArray = new Vector3[4];
        RectCorner cachedRectCorner;
        float lerpZPosition = 0;
        RectCorner lerpWorldCorners;
        Quaternion lerpRotation;
        public override void SetDirty()
        {
            if (targetRectTransform == null)
                return;

            Quaternion temp = targetRectTransform.rotation;

            targetRectTransform.rotation = Quaternion.identity;
            targetRectTransform.GetWorldCorners(fourCornersArray);
            targetRectTransform.rotation = temp;

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

            lerpZPosition = lerpZPosition.Lerp(targetRectTransform.position.z, currentLerpSpeed);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, lerpZPosition);

            rectTransform.rotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);

            rectTransform.pivot = rectTransform.pivot.Lerp(targetRectTransform.pivot, currentLerpSpeed);

            lerpWorldCorners = lerpWorldCorners.Lerp(cachedRectCorner, currentLerpSpeed);
            worldCorners = lerpWorldCorners;

            lerpRotation = lerpRotation.Lerp(targetRectTransform.rotation, currentLerpSpeed);
            rectTransform.rotation = lerpRotation;
        }
    }
}
