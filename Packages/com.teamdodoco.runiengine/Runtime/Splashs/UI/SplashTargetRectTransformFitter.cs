#nullable enable
using UnityEngine;

namespace RuniEngine.Splashs.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class SplashTargetRectTransformFitter : MonoBehaviour
    {
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null || _rectTransform.gameObject != gameObject)
                {
                    if (transform is RectTransform rectTransform)
                        _rectTransform = rectTransform;
                    else
                        _rectTransform = gameObject.AddComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        RectTransform? _rectTransform;

        public RectCorner worldCorners
        {
            get
            {
                rectTransform.GetWorldCorners(worldCornersArray);
                return new RectCorner(worldCornersArray[0], worldCornersArray[1], worldCornersArray[2], worldCornersArray[3]);
            }
            set
            {
                Rect rect = value;
                Matrix4x4 matrix4x = rectTransform.worldToLocalMatrix;

                Vector2 position = matrix4x.MultiplyVector(rect.min);
                Vector2 size = (Vector2)matrix4x.MultiplyVector(rect.max) - position;

                {
                    Vector2 temp = (position + (size * rectTransform.pivot)) * rectTransform.lossyScale;
                    rectTransform.position = new Vector3(temp.x, temp.y, rectTransform.position.z);
                }

                rectTransform.sizeDelta = size;
            }
        }
        readonly Vector3[] worldCornersArray = new Vector3[4];

        public RectTransform? targetRectTransform { get => _targetRectTransform; set => _targetRectTransform = value; }
        [SerializeField, NotNullField] RectTransform? _targetRectTransform;



        void Awake() => Canvas.preWillRenderCanvases += SetDirty;
        void OnDestroy()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();

            Canvas.preWillRenderCanvases -= SetDirty;
        }

        DrivenRectTransformTracker tracker;

        readonly Vector3[] fourCornersArray = new Vector3[4];
        RectCorner cachedRectCorner;
        public void SetDirty()
        {
            if (targetRectTransform == null)
                return;

            Quaternion temp = targetRectTransform.rotation;

            targetRectTransform.rotation = Quaternion.identity;
            targetRectTransform.GetWorldCorners(fourCornersArray);
            targetRectTransform.rotation = temp;

            cachedRectCorner = new RectCorner(fourCornersArray[0], fourCornersArray[1], fourCornersArray[2], fourCornersArray[3]);
            LayoutUpdate();
        }

        public void LayoutUpdate()
        {
            if (targetRectTransform == null)
                return;

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, rectTransform, DrivenTransformProperties.All);
            }

            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, targetRectTransform.position.z);

            rectTransform.rotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);

            rectTransform.pivot = targetRectTransform.pivot;

            worldCorners = cachedRectCorner;

            rectTransform.rotation = targetRectTransform.rotation;
        }
    }
}
