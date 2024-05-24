#nullable enable
using UnityEngine;

namespace RuniEngine.UI
{
    [ExecuteAlways]
    public class SplashScreenProgressBar : MonoBehaviour
    {
        public float progress { get => _progress; set => _progress = value; }
        [SerializeField, Range(0, 1)] float _progress = 0;

        public bool right { get => _right; set => _right = value; }
        [SerializeField] bool _right = false;

        public bool allowNoResponseAni { get => _allowNoResponseAni; set => _allowNoResponseAni = value; }
        [SerializeField] bool _allowNoResponseAni = true;

        public RectTransform? fill => _fill;
        [SerializeField] RectTransform? _fill;

        public bool isNoResponse { get; protected set; } = false;

        protected virtual void OnEnable() => Initialize();
        protected virtual void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();

            Initialize();
        }

        float loopValue = 0;
        float lastProgress = 0;
        float noResponseTimer = 0;
        float lastMinX = 0;
        float lastMaxX = 0;

        float anchorMinX = 0;
        float anchorMaxX = 0;
        DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();
        protected virtual void Update()
        {
            float lerpValue = 0.2f;

            if (!Kernel.isPlaying && fill != null)
            {
                tracker.Clear();
                tracker.Add(this, fill, DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX);
            }

            if (Kernel.isPlaying && noResponseTimer >= 2 && allowNoResponseAni && progress < 1)
            {
                if (!isNoResponse)
                {
                    if (right)
                    {
                        lastMinX = anchorMinX - loopValue.Clamp01();
                        lastMaxX = anchorMaxX - (loopValue + 0.25f).Clamp01();
                    }
                    else
                    {
                        lastMinX = anchorMinX - (loopValue - 0.25f).Clamp01();
                        lastMaxX = anchorMaxX - loopValue.Clamp01();
                    }

                    isNoResponse = Kernel.isPlaying;
                }

                if (right)
                    loopValue -= 0.0125f * Kernel.fpsUnscaledSmoothDeltaTime;
                else
                    loopValue += 0.0125f * Kernel.fpsUnscaledSmoothDeltaTime;

                lastMinX = lastMinX.Lerp(0, lerpValue);
                lastMaxX = lastMaxX.Lerp(0, lerpValue);

                if (right)
                {
                    anchorMinX = (loopValue + lastMinX).Clamp01();
                    anchorMaxX = (loopValue + 0.25f + lastMaxX).Clamp01();
                }
                else
                {
                    anchorMinX = (loopValue - 0.25f + lastMinX).Clamp01();
                    anchorMaxX = (loopValue + lastMaxX).Clamp01();
                }

                if (right)
                {
                    if (anchorMaxX <= 0)
                        loopValue = 1;
                }
                else
                {
                    if (anchorMinX >= 1)
                        loopValue = 0;
                }
            }
            else
            {
                if (Kernel.isPlaying)
                {
                    isNoResponse = false;
                    noResponseTimer += Kernel.unscaledDeltaTime;
                }

                if (right)
                {
                    anchorMinX = anchorMinX.Lerp(1 - progress, lerpValue);
                    anchorMaxX = anchorMaxX.Lerp(1, lerpValue);
                }
                else
                {
                    anchorMinX = anchorMinX.Lerp(0, lerpValue);
                    anchorMaxX = anchorMaxX.Lerp(progress, lerpValue);
                }
            }

            if (fill != null)
            {
                fill.anchorMin = new Vector2(anchorMinX, fill.anchorMin.y);
                fill.anchorMax = new Vector2(anchorMaxX, fill.anchorMax.y);
            }

            if (Kernel.isPlaying && lastProgress != progress)
            {
                noResponseTimer = 0;
                lastProgress = progress;
            }
        }

        public void Initialize()
        {
            loopValue = 0;
            isNoResponse = false;
            lastProgress = 0;
            noResponseTimer = 0;
            lastMinX = 0;
            lastMaxX = 0;

            anchorMinX = 0;
            anchorMaxX = 0;

            if (fill != null)
            {
                fill.anchorMin = new Vector2(0, fill.anchorMin.y);
                fill.anchorMax = new Vector2(0, fill.anchorMax.y);
            }
        }
    }
}