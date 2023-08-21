#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuniEngine
{
    public static class ResourceManager
    {
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
