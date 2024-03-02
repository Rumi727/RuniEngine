#nullable enable
using RuniEngine.Screens;
using System;
using UnityEngine;

namespace RuniEngine
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraSetter : MonoBehaviour
    {
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

        void OnPreCull() => SetDirty();

        public void SetDirty()
        {
            if (camera == null)
                return;
            if (disable)
                return;

            Rect rect = normalizedViewPortRect;
            rect.position += (Vector2)ScreenManager.screenPosition * 2 * globalScreenPositionMultiple / ScreenManager.size;

            rect.min += ScreenManager.screenArea.min / ScreenManager.size;
            rect.max += ScreenManager.screenArea.max / ScreenManager.size;

            rect.size += rect.position.Clamp(new Vector2(float.MinValue, float.MinValue), Vector2.zero);
            rect.position = rect.position.Clamp(Vector2.zero, new Vector2(0.9999999f, 0.9999999f));

            rect.size = rect.size.Clamp(new Vector2(0.0001f, 0.0001f), Vector2.positiveInfinity);

            camera.rect = rect;
        }
    }
}