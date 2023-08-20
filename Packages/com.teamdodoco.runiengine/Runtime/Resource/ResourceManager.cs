#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuniEngine
{
    public static class ResourceManager
    {
        #region Default Objects
        /// <summary>
        /// 빈 게임 오브젝트
        /// </summary>
        public static Transform emptyTransform
        {
            get
            {
                if (_emptyTransform == null)
                    _emptyTransform = Resources.Load<Transform>("Empty Transform");

                return _emptyTransform;
            }
        }
        static Transform? _emptyTransform;

        /// <summary>
        /// 사각 트랜스폼이 추가된 빈 게임 오브젝트
        /// </summary>
        public static RectTransform emptyRectTransform
        {
            get
            {
                if (_emptyRectTransform == null)
                    _emptyRectTransform = Resources.Load<RectTransform>("Empty Rect Transform");

                return _emptyRectTransform;
            }
        }
        static RectTransform? _emptyRectTransform;



        /// <summary>
        /// 기본 메테리얼
        /// </summary>
        public static Material defaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                    _defaultMaterial = Resources.Load<Material>("Default Material");

                return _defaultMaterial;
            }
        }
        static Material? _defaultMaterial;

        /// <summary>
        /// 단색 메테리얼
        /// </summary>
        public static Material coloredMaterial
        {
            get
            {
                if (_coloredMaterial == null)
                {
                    Shader shader = Shader.Find("Hidden/Internal-Colored");
                    _coloredMaterial = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    _coloredMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _coloredMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _coloredMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _coloredMaterial.SetInt("_ZWrite", 0);

                    allLoadedResources.Add(_coloredMaterial);
                    allLoadedResources.Add(_coloredMaterial.shader);
                }

                return _coloredMaterial;
            }
        }
        static Material? _coloredMaterial;
        #endregion

        public static List<Object> allLoadedResources { get; } = new();

        /// <summary>
        /// 모든 리소스를 삭제합니다
        /// </summary>
        public static void AllDestroy()
        {
            List<Sprite> allLoadedSprite = allLoadedResources.OfType<Sprite>().ToList();
            for (int i = 0; i < allLoadedSprite.Count; i++)
            {
                Sprite sprite = allLoadedSprite[i];
                if (sprite != null)
                    Object.DestroyImmediate(sprite);
            }

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object resource = allLoadedResources[i];
                if (resource != null)
                    Object.DestroyImmediate(resource);
            }

            allLoadedResources.Clear();
        }
    }
}
