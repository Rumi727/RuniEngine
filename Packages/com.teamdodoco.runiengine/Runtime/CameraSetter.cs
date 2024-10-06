using RuniEngine.Screens;
using UnityEngine;

namespace RuniEngine
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraSetter : MonoBehaviour
    {
        //new 키워드 추가시 빌드 오류 발생
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        public Camera? camera => _camera = this.GetComponentFieldSave(_camera, GetComponentMode.destroyIfNull);
        Camera? _camera;
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.

        public Rect normalizedViewPortRect
        {
            get => _normalizedViewPortRect;
            set
            {
                _normalizedViewPortRect = value;
                SetDirty();
            }
        }
        [SerializeField] Rect _normalizedViewPortRect = new Rect(0, 0, 1, 1);

        public Vector2 globalScreenPositionMultiple
        {
            get => _globalScreenPositionMultiple;
            set
            {
                _globalScreenPositionMultiple = value;
                SetDirty();
            }
        }
        [SerializeField] Vector2 _globalScreenPositionMultiple = Vector2.one;
        public Vector2 globalScreenAreaMultiple
        {
            get => _globalScreenAreaMultiple;
            set
            {
                _globalScreenAreaMultiple = value;
                SetDirty();
            }
        }
        [SerializeField] Vector2 _globalScreenAreaMultiple = Vector2.one;

        /// <summary>
        /// 스크립트가 카메라의 설정을 변경하지 못하게 막습니다
        /// </summary>
        public bool disable { get => _disable; set => _disable = value; }
        [SerializeField] bool _disable;

        void OnEnable() => SetDirty();
        void OnPreCull() => SetDirty();
        void LateUpdate() => SetDirty();

        bool isProjectionReset = false;
        public void SetDirty()
        {
            if (camera == null)
                return;
            if (disable)
                return;

            Rect rect = normalizedViewPortRect;
            rect.position += (Vector2)ScreenManager.screenPosition * globalScreenPositionMultiple / ScreenManager.size;

            rect.min += ScreenManager.screenArea.min / ScreenManager.size;
            rect.max += ScreenManager.screenArea.max / ScreenManager.size;

            if (rect.position.x < 0 || rect.position.x + rect.width > 1 || rect.position.y < 0 || rect.position.y + rect.height > 1)
            {
                isProjectionReset = false;
                camera.ResetProjectionMatrix();

                float x = 0;
                float y = 0;
                float size = 1;

                Matrix4x4 m = camera.projectionMatrix;
                if (rect.position.x < 0 && rect.position.x + rect.width > 1)
                    x = (rect.position.x * 2) + (rect.width - 1);
                else if (rect.position.x + rect.width > 1)
                    x = (rect.position.x + rect.width - 1).Clamp(0) / (-rect.position.x + 1).Clamp(MathUtility.epsilonFloatWithAccuracy);
                else if (rect.position.x < 0)
                    x = (rect.position.x) / (rect.position.x + rect.width).Clamp(MathUtility.epsilonFloatWithAccuracy);

                if (rect.position.y < 0 && rect.position.y + rect.height > 1)
                {
                    y = (rect.position.y * 2) + (rect.height - 1);
                    size = 1 / rect.height;
                }
                else if (rect.position.y + rect.height > 1)
                {
                    float yHeight = rect.position.y + rect.height;

                    y = (yHeight - 1).Clamp(0) / (-rect.position.y + 1).Clamp(MathUtility.epsilonFloatWithAccuracy);
                    size = (1 - ((yHeight - 1) / rect.height)).Clamp(MathUtility.epsilonFloatWithAccuracy, 1);
                }
                else if (rect.position.y < 0)
                {
                    y = rect.position.y / (rect.position.y + rect.height).Clamp(MathUtility.epsilonFloatWithAccuracy);
                    size = ((rect.position.y + rect.height) / rect.height).Clamp(MathUtility.epsilonFloatWithAccuracy);
                }

                if (camera.orthographic)
                {
                    m[0, 3] = x;
                    m[1, 3] = y;
                }
                else
                {
                    m[0, 2] = -x;
                    m[1, 2] = -y;
                }

                m[0, 0] /= size;
                m[1, 1] /= size;

                camera.projectionMatrix = m;
            }
            else if (!isProjectionReset)
            {
                isProjectionReset = true;
                camera.ResetProjectionMatrix();
            }

            rect.size += rect.position.Clamp(new Vector2(float.MinValue, float.MinValue), Vector2.zero);
            rect.position = rect.position.Clamp(Vector2.zero, new Vector2(1 - (1f / ScreenManager.width), 1 - (1f / ScreenManager.height)));

            rect.size = rect.size.Clamp(new Vector2(1f / ScreenManager.width, 1f / ScreenManager.height), Vector2.positiveInfinity);

            camera.rect = rect;
        }
    }
}
