using UnityEngine;
using RuniEngine.Screens;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuniEngine.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasSetter : UIBase
    {
        public bool disableScreenArea
        {
            get => _disableScreenArea;
            set
            {
                _disableScreenArea = value;
                SetDirty();
            }
        }
        [SerializeField] bool _disableScreenArea = false;



        public bool worldRenderMode
        {
            get => _worldRenderMode;
            set
            {
                _worldRenderMode = value;
                SetDirty();
            }
        }
        [SerializeField] bool _worldRenderMode = false;

        public float planeDistance
        {
            get => _planeDistance;
            set
            {
                _planeDistance = value;
                SetDirty();
            }
        }
        [SerializeField] float _planeDistance = 14;



        public RectOffset areaOffset
        {
            get => _areaOffset;
            set
            {
                _areaOffset = value;
                SetDirty();
            }
        }
        [SerializeField] RectOffset _areaOffset = RectOffset.zero;



        public Vector3 globalScreenPositionMultiple
        {
            get => _globalScreenPositionMultiple;
            set
            {
                _globalScreenPositionMultiple = value;
                SetDirty();
            }
        }
        [SerializeField] Vector3 _globalScreenPositionMultiple = Vector3.one;

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



        public float uiSize
        {
            get => _uiSize;
            set
            {
                _uiSize = value;
                SetDirty();
            }
        }
        [SerializeField] float _uiSize = 1;

        public bool disableUISize
        {
            get => _disableUISize;
            set
            {
                _disableUISize = value;
                SetDirty();
            }
        }
        [SerializeField] bool _disableUISize = false;



        /// <summary>
        /// 이 변수를 활성화 하면 에디터에서 씬 가시성이 항상 활성화 됩니다.
        /// 이 프로퍼티는 런타임에 영향을 미치지 않습니다.
        /// </summary>
        public bool alwaysVisible { get => _alwaysVisible; set => _alwaysVisible = value; }
        [Tooltip("inspector.canvas_setter.alwaysVisible.tooltip"), SerializeField] bool _alwaysVisible = false;




        public RectTransform? areaObject => _areaObject;
        [SerializeField, HideInInspector] RectTransform? _areaObject;



        DrivenRectTransformTracker tracker;



        protected override void OnEnable() => Canvas.preWillRenderCanvases += SetDirty;
        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();

            Canvas.preWillRenderCanvases -= SetDirty;
        }



        public void SetDirty()
        {
            if (canvas == null)
                return;

            if (!disableUISize)
            {
                if (Kernel.isPlaying)
                    canvas.scaleFactor = UIManager.uiSize * uiSize;
                else
                    canvas.scaleFactor = uiSize;
            }

            if (!disableScreenArea)
            {
                SafeScreenSetting(canvas.renderMode == RenderMode.ScreenSpaceOverlay);

                if (worldRenderMode)
                    WorldRenderCamera();
            }
            else
                SafeScreenDestroy();
        }

        void SafeScreenSetting(bool useScreenArea)
        {
            if (canvas == null)
                return;

            if (areaObject == null)
            {
                _areaObject = Instantiate(ResourceUtility.emptyRectTransform, transform.parent);
                if (areaObject == null)
                    return;

                areaObject.name = "Area";
            }

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, areaObject, DrivenTransformProperties.All);
            }

#if UNITY_EDITOR
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(areaObject.gameObject);
                if (prefabAssetType == PrefabAssetType.NotAPrefab)
                {
                    if (areaObject.parent != transform)
                        areaObject.SetParent(transform);
                }
            }
#else
            if (areaObject.parent != transform)
                areaObject.SetParent(transform);
#endif

            areaObject.anchorMin = Vector2.zero;
            areaObject.anchorMax = Vector2.one;

            if (useScreenArea)
            {
                areaObject.offsetMin = ScreenManager.screenArea.min * globalScreenAreaMultiple / canvas.scaleFactor;
                areaObject.offsetMax = ScreenManager.screenArea.max * globalScreenAreaMultiple / canvas.scaleFactor;

                areaObject.position = new Vector3(areaObject.position.x, areaObject.position.y, 0);
                areaObject.position += ScreenManager.screenPosition.Multiply(globalScreenPositionMultiple) / canvas.scaleFactor;
            }
            else
            {
                areaObject.offsetMin = Vector2.zero;
                areaObject.offsetMax = Vector2.zero;
            }

            areaObject.offsetMin += areaOffset.min / canvas.scaleFactor;
            areaObject.offsetMax += areaOffset.max / canvas.scaleFactor;

            areaObject.pivot = Vector2.zero;

            areaObject.localEulerAngles = Vector3.zero;
            areaObject.localScale = Vector3.one;

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childtransform = transform.GetChild(i);
                if (childtransform != areaObject)
                {
#if UNITY_EDITOR
                    {
                        PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(childtransform.gameObject);
                        if (prefabAssetType == PrefabAssetType.NotAPrefab)
                            childtransform.SetParent(areaObject);
                    }
#else
                    childtransform.SetParent(areaObject);
#endif
                    i--;
                    childCount--;
                }
            }
        }

        void SafeScreenDestroy()
        {
            if (areaObject == null)
                return;

            int childCount = areaObject.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childtransform = areaObject.GetChild(i);
                if (childtransform != areaObject)
                {
#if UNITY_EDITOR
                    {
                        PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(childtransform.gameObject);
                        if (prefabAssetType == PrefabAssetType.NotAPrefab)
                            childtransform.SetParent(transform);
                    }
#else
                    childtransform.SetParent(transform);
#endif
                    i--;
                    childCount--;
                }
            }

#if UNITY_EDITOR
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(areaObject.gameObject);
                if (prefabAssetType == PrefabAssetType.NotAPrefab)
                    DestroyImmediate(areaObject.gameObject);
            }
#else
            DestroyImmediate(areaObject.gameObject);
#endif

            _areaObject = null;
        }

        void WorldRenderCamera()
        {
            if (canvas == null)
                return;

            if (!Kernel.isPlaying)
                tracker.Add(this, rectTransform, DrivenTransformProperties.All);

            canvas.renderMode = RenderMode.WorldSpace;

            Camera? camera = canvas.worldCamera;
            if (camera == null)
                return;

            transform.SetPositionAndRotation(camera.transform.position + (transform.forward * planeDistance), camera.transform.rotation);

            float uiSize;
            if (disableUISize)
                uiSize = canvas.scaleFactor;
            else
                uiSize = UIManager.uiSize * this.uiSize;

            uiSize = uiSize.Clamp(0.0001f);

            float width = camera.pixelWidth * (1 / uiSize);
            float height = camera.pixelHeight * (1 / uiSize);

            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.pivot = Vector2.one * 0.5f;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;



            float screenX, screenY;

            if (camera.orthographic)
            {
                screenY = camera.orthographicSize * 2;
                screenX = screenY / height * width;
            }
            else
            {
                screenY = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * planeDistance;
                screenX = screenY / height * width;
            }

            transform.localScale = new Vector3(screenX / width, screenY / height, screenX / width);
        }
    }
}