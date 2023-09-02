#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Threading;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using RuniEngine.Json;
using System.Collections.Generic;

namespace RuniEngine.Resource
{
    public sealed class ImageLoader : IResourceElement
    {
        public const string name = "textures";
        string IResourceElement.name => name;

        public const string spriteDefaultTag = "global";

        #region Get Texture
        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 가져옵니다
        /// Import image files as Texture2D type
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷
        /// </param>
        /// <returns></returns>
        public static Texture2D? GetTexture(string path, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(path + ".json") ?? new TextureMetaData();
            return GetTexture(path, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);
        }

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 가져옵니다
        /// Import image files as Texture2D type
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷
        /// </param>
        /// <returns></returns>
        public static Texture2D? GetTexture(string path, TextureMetaData textureMetaData, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave) => GetTexture(path, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 가져옵니다
        /// Import image files as Texture2D type
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷 (png, jpg 파일에서만 작동)
        /// </param>
        /// <returns></returns>
        public static Texture2D? GetTexture(string path, FilterMode filterMode, bool generateMipmap, TextureCompressionQuality compressionType, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            if (File.Exists(path))
            {
                Texture2D texture = new Texture2D(0, 0, textureFormat, generateMipmap, false);
                ResourceManager.allLoadedResources.Add(texture);

                texture.filterMode = filterMode;
                texture.name = Path.GetFileNameWithoutExtension(path);
                texture.hideFlags = hideFlags;
                texture.mipMapBias = -0.5f;

                AsyncImageLoader.LoaderSettings loaderSettings = AsyncImageLoader.LoaderSettings.Default;
                loaderSettings.generateMipmap = generateMipmap;
                loaderSettings.logException = true;

                if (!AsyncImageLoader.LoadImage(texture, File.ReadAllBytes(path), loaderSettings))
                    return null;

                if (compressionType == TextureCompressionQuality.normal)
                    texture.Compress(false);
                else if (compressionType == TextureCompressionQuality.highQuality)
                    texture.Compress(true);

                return texture;
            }

            return null;
        }
        #endregion

        #region Get Texture Async
        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 비동기로 가져옵니다
        /// 다양한 포맷을 지원하며 이중엔 SC KRM이 지원하는 포맷과 유니티가 지원하는 포맷도 있습니다
        /// Asynchronously import an image file as a Texture2D type.
        /// Various formats are supported. Among them, there are formats supported by SC KRM and formats supported by Unity.
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷 (png, jpg 파일에서만 작동)
        /// </param>
        /// <returns></returns>
        public static UniTask<Texture2D?> GetTextureAsync(string path, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            TextureMetaData? textureMetaData = JsonManager.JsonRead<TextureMetaData>(path + ".json") ?? new TextureMetaData();
            return GetTextureAsync(path, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);
        }

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 비동기로 가져옵니다
        /// 다양한 포맷을 지원하며 이중엔 SC KRM이 지원하는 포맷과 유니티가 지원하는 포맷도 있습니다
        /// Asynchronously import an image file as a Texture2D type.
        /// Various formats are supported. Among them, there are formats supported by SC KRM and formats supported by Unity.
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷
        /// </param>
        /// <returns></returns>
        public static UniTask<Texture2D?> GetTextureAsync(string path, TextureMetaData textureMetaData, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave) => GetTextureAsync(path, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 비동기로 가져옵니다
        /// 다양한 포맷을 지원하며 이중엔 SC KRM이 지원하는 포맷과 유니티가 지원하는 포맷도 있습니다
        /// Asynchronously import an image file as a Texture2D type.
        /// Various formats are supported. Among them, there are formats supported by SC KRM and formats supported by Unity.
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷
        /// </param>
        /// <returns></returns>
        public static async UniTask<Texture2D?> GetTextureAsync(string path, FilterMode filterMode, bool generateMipmap, TextureCompressionQuality compressionType, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            if (File.Exists(path))
            {
#if (UNITY_STANDALONE_LINUX && !UNITY_EDITOR) || UNITY_EDITOR_LINUX
                byte[] textureBytes = File.ReadAllBytes(path);
#else
                using UnityWebRequest www = UnityWebRequest.Get(path.UrlPathPrefix());
                await www.SendWebRequest();

                if (!Kernel.isPlaying)
                    return null;

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError(www.error);

                byte[] textureBytes = www.downloadHandler.data;
#endif

                Texture2D texture = new Texture2D(0, 0, textureFormat, generateMipmap);
                ResourceManager.allLoadedResources.Add(texture);

                texture.filterMode = filterMode;
                texture.name = Path.GetFileNameWithoutExtension(path);
                texture.mipMapBias = -0.5f;

                AsyncImageLoader.LoaderSettings loaderSettings = AsyncImageLoader.LoaderSettings.Default;
                loaderSettings.generateMipmap = generateMipmap;
                loaderSettings.logException = true;

                texture.hideFlags = HideFlags.DontUnloadUnusedAsset;

                if (!await AsyncImageLoader.LoadImageAsync(texture, textureBytes, loaderSettings) || !Kernel.isPlaying)
                {
                    Object.DestroyImmediate(texture);
                    return null;
                }

                ResourceManager.allLoadedResources.Add(texture);

                texture.hideFlags = hideFlags;

                if (compressionType == TextureCompressionQuality.normal)
                    texture.Compress(false);
                else if (compressionType == TextureCompressionQuality.highQuality)
                    texture.Compress(true);

                return texture;
            }

            return null;
        }
        #endregion

        #region Get Sprite
        /// <summary>
        /// 텍스쳐를 스프라이트로 변환합니다 (Unity API를 사용하기 때문에 메인 스레드에서 실행해야 합니다)
        /// Convert texture to sprite (Since the Unity API is used, we need to run it on the main thread)
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        public static Sprite GetSprite(Texture2D texture, SpriteMetaData? spriteMetaData = null, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            if (spriteMetaData == null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero);
                sprite.name = texture.name;
                sprite.hideFlags = hideFlags;

                ResourceManager.allLoadedResources.Add(sprite);

                return sprite;
            }
            else
            {
                spriteMetaData.RectMinMax(texture.width, texture.height);
                spriteMetaData.PixelsPreUnitMinSet();

                Sprite sprite = Sprite.Create(texture, spriteMetaData.rect, spriteMetaData.pivot, spriteMetaData.pixelsPerUnit, 0, SpriteMeshType.FullRect, spriteMetaData.border);
                sprite.name = texture.name;
                sprite.hideFlags = hideFlags;

                ResourceManager.allLoadedResources.Add(sprite);

                return sprite;
            }
        }

        /// <summary>
        /// 이미지 파일을 스프라이트로 가져옵니다 (Unity API를 사용하기 때문에 메인 스레드에서 실행해야 합니다.)
        /// Import image files as sprites (Since the Unity API is used, we need to run it on the main thread)
        /// </summary>
        /// <param name="path">
        /// 이미지 파일의 경로
        /// Path
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// Use extension in path
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        public static Dictionary<string, Sprite[]>? GetSprites(string path, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            Texture2D? texture = GetTexture(path, textureFormat);
            if (texture == null)
                return null;

            Dictionary<string, SpriteMetaData[]>? spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(path + ".json");
            spriteMetaDatas ??= new Dictionary<string, SpriteMetaData[]>();

            return GetSprites(texture, hideFlags, spriteMetaDatas);
        }

        /// <summary>
        /// 텍스쳐를 스프라이트로 변환합니다 (Unity API를 사용하기 때문에 메인 스레드에서 실행해야 합니다)
        /// Import image files as sprites (Since the Unity API is used, we need to run it on the main thread)
        /// </summary>
        /// <param name="texture">
        /// 변환할 텍스쳐
        /// texture to convert
        /// </param>
        /// <param name="spriteMetaDatas">
        /// Sprite's metadata
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        public static Dictionary<string, Sprite[]> GetSprites(Texture2D texture, HideFlags hideFlags, Dictionary<string, SpriteMetaData[]> spriteMetaDatas)
        {
            NotMainThreadException.Exception();

            if (!spriteMetaDatas.ContainsKey(spriteDefaultTag))
                spriteMetaDatas.Add(spriteDefaultTag, new SpriteMetaData[] { new SpriteMetaData() });

            Dictionary<string, Sprite[]> sprites = new Dictionary<string, Sprite[]>();
            foreach (var item in spriteMetaDatas)
            {
                SpriteMetaData[] spriteMetaDatas2 = item.Value;
                sprites.Add(item.Key, new Sprite[spriteMetaDatas2.Length]);

                for (int i = 0; i < spriteMetaDatas2.Length; i++)
                {
                    SpriteMetaData spriteMetaData = spriteMetaDatas2[i];
                    spriteMetaData ??= new SpriteMetaData();

                    spriteMetaData.RectMinMax(texture.width, texture.height);
                    spriteMetaData.PixelsPreUnitMinSet();

                    Sprite sprite = Sprite.Create(texture, spriteMetaData.rect, spriteMetaData.pivot, spriteMetaData.pixelsPerUnit, 0, SpriteMeshType.FullRect, spriteMetaData.border);
                    sprite.name = texture.name;
                    sprite.hideFlags = hideFlags;

                    ResourceManager.allLoadedResources.Add(sprite);
                    sprites[item.Key][i] = sprite;
                }
            }
            return sprites;
        }
        #endregion



        public void Apply() => throw new System.NotImplementedException();

        public UniTask Refresh(string nameSpacePath, string nameSpace) => throw new System.NotImplementedException();

        public void Clear() => throw new System.NotImplementedException();
    }
}
