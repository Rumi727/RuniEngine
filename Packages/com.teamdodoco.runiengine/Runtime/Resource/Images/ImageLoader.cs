#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Threading;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using RuniEngine.Json;
using System.Collections.Generic;
using RuniEngine.Booting;
using System.Linq;

namespace RuniEngine.Resource.Images
{
    public sealed class ImageLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        /// <summary>
        /// Texture2D = allTextures[nameSpace][type];
        /// </summary>
        static Dictionary<string, Dictionary<string, Texture2D>> packTextures = new();
        /// <summary>
        /// Rect = allTextureRects[nameSpace][type][fileName];
        /// </summary>
        static Dictionary<string, Dictionary<string, Dictionary<string, Rect>>> packTextureRects = new();
        /// <summary>
        /// string = allTexturePaths[nameSpace][type][fileName];
        /// </summary>
        static Dictionary<string, Dictionary<string, Dictionary<string, string>>> packTexturePaths = new();
        /// <summary>
        /// string = allTexturePaths[nameSpace][type];
        /// </summary>
        static Dictionary<string, Dictionary<string, string>> packTextureTypePaths = new();
        /// <summary>
        /// Sprite = allTextureSprites[nameSpace][type][fileName];
        /// </summary>
        static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>> allTextureSprites = new();



        public const string name = "textures";
        string IResourceElement.name => name;

        public const string spriteDefaultTag = "global";



        [Awaken]
        static void Awaken() => ResourceManager.ElementRegister(new ImageLoader());



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

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(path + ".json") ?? new TextureMetaData();
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

                Texture2D texture = new Texture2D(1, 1, textureFormat, generateMipmap);
                ResourceManager.allLoadedResources.Add(texture);

                texture.filterMode = filterMode;
                texture.name = Path.GetFileNameWithoutExtension(path);
                texture.mipMapBias = -0.5f;

                AsyncImageLoader.LoaderSettings loaderSettings = AsyncImageLoader.LoaderSettings.Default;
                loaderSettings.generateMipmap = generateMipmap;
                loaderSettings.logException = true;

                texture.hideFlags = HideFlags.DontSave;
                
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

        public static string SearchTexturePath(string type, string name, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (packTexturePaths.ContainsKey(nameSpace) && packTexturePaths[nameSpace].ContainsKey(type) && packTexturePaths[nameSpace][type].ContainsKey(name))
                return packTexturePaths[nameSpace][type][name];

            return "";
        }

        public static string SearchTextureTypePath(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (packTextureTypePaths.ContainsKey(nameSpace) && packTextureTypePaths[nameSpace].ContainsKey(type))
                return packTextureTypePaths[nameSpace][type];

            return "";
        }

        public static Texture2D? SearchPackTexture(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (packTextures.ContainsKey(nameSpace) && packTextures[nameSpace].ContainsKey(type))
                return packTextures[nameSpace][type];

            return null;
        }

        public static Rect SearchTextureRect(string type, string name, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (packTextureRects.ContainsKey(nameSpace) && packTextureRects[nameSpace].ContainsKey(type) && packTextureRects[nameSpace][type].ContainsKey(name))
                return packTextureRects[nameSpace][type][name];

            return Rect.zero;
        }

        public static Sprite[]? SearchSprites(string type, string name, string nameSpace = "", string tag = spriteDefaultTag)
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (allTextureSprites.ContainsKey(nameSpace) && allTextureSprites[nameSpace].ContainsKey(type) && allTextureSprites[nameSpace][type].ContainsKey(name))
            {
                if (allTextureSprites[nameSpace][type][name].ContainsKey(tag))
                    return allTextureSprites[nameSpace][type][name][tag];
                else if (allTextureSprites[nameSpace][type][name].ContainsKey(spriteDefaultTag))
                    return allTextureSprites[nameSpace][type][name][spriteDefaultTag];
            }

            return null;
        }




        public async UniTask Load()
        {
            NotPlayModeException.Exception();

            /// <summary>
            /// Texture2D = tempTextures[nameSpace][type][name];
            /// </summary>
            Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>> tempTextures = new();
            Dictionary<string, Dictionary<string, Texture2D>> tempPackTextures = new();
            Dictionary<string, Dictionary<string, Dictionary<string, Rect>>> tempPackTextureRects = new();
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempPackTexturePaths = new();
            Dictionary<string, Dictionary<string, string>> tempPackTextureTypePaths = new();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>> tempAllTextureSprites = new();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(() => ResourceManager.ResourcePackLoop(FindTextures));
            else
                await ResourceManager.ResourcePackLoop(FindTextures);

            if (!Kernel.isPlaying)
                return;

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(PackTextures);
            else
                await PackTextures();

            if (!Kernel.isPlaying)
                return;

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(LoadSprite);
            else
                await LoadSprite();

            if (!Kernel.isPlaying)
                return;

            foreach (var item2 in from item in packTextures from item2 in item.Value select item2)
                ResourceManager.garbages.Add(item2.Value);

            foreach (var item4 in from item in allTextureSprites from item2 in item.Value from item3 in item2.Value from item4 in item3.Value select item4)
            {
                for (int i = 0; i < item4.Value.Length; i++)
                    ResourceManager.garbages.Add(item4.Value[i]);
            }

            packTextures = tempPackTextures;
            packTextureRects = tempPackTextureRects;
            packTexturePaths = tempPackTexturePaths;
            packTextureTypePaths = tempPackTextureTypePaths;
            allTextureSprites = tempAllTextureSprites;

            isLoaded = true;

            async UniTask FindTextures(string nameSpacePath, string nameSpace)
            {
                string rootPath = Path.Combine(nameSpacePath, name);
                if (!Directory.Exists(rootPath))
                    return;

                List<UniTask> tasks = new List<UniTask>();
                string[] typePaths = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

                for (int i = 0; i < typePaths.Length; i++)
                {
                    string typePath = typePaths[i];
                    string localTypePath = typePath.Substring(rootPath.Length + 1);

                    string[] filePaths = DirectoryUtility.GetFiles(typePath, ExtensionFilter.pictureFileFilter);
                    for (int j = 0; j < filePaths.Length; j++)
                    {
                        tasks.Add(Task());

                        //병렬 로드
                        async UniTask Task()
                        {
                            string filePath = filePaths[i].UniformDirectorySeparatorCharacter();
                            string fileName = Path.GetFileName(filePath);

                            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(typePath + ".json") ?? new TextureMetaData();
                            Texture2D? texture = await await ThreadDispatcher.Execute(() => GetTextureAsync(filePath, textureMetaData));
                            if (!Kernel.isPlaying || texture == null)
                                return;

                            tempPackTexturePaths.TryAdd(nameSpace, new());
                            tempPackTexturePaths[nameSpace].TryAdd(localTypePath, new());
                            tempPackTexturePaths[nameSpace][localTypePath].TryAdd(fileName, filePath);

                            tempPackTextureTypePaths.TryAdd(nameSpace, new());
                            tempPackTextureTypePaths[nameSpace].TryAdd(localTypePath, typePath);

                            tempTextures.TryAdd(nameSpace, new());
                            tempTextures[nameSpace].TryAdd(localTypePath, new());
                            tempTextures[nameSpace][localTypePath].TryAdd(fileName, texture);
                        }
                    }
                }

                await UniTask.WhenAll(tasks);
                if (!Kernel.isPlaying)
                    return;
            }

            async UniTask PackTextures()
            {
                foreach (var nameSpaces in tempTextures)
                {
                    /* packTextureRects */
                    Dictionary<string, Dictionary<string, Rect>> type_name_rect = new Dictionary<string, Dictionary<string, Rect>>();
                    /* packTextures */
                    Dictionary<string, Texture2D> type_texture = new Dictionary<string, Texture2D>();

                    foreach (var types in nameSpaces.Value)
                    {
                        string[] textureNames = types.Value.Keys.ToArray();
                        Texture2D[] textures = types.Value.Values.ToArray();
                        Texture2D? background = null;
                        Rect[]? rects = null;

                        TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(tempPackTextureTypePaths[nameSpaces.Key][types.Key] + ".json") ?? new TextureMetaData();
                        await ThreadDispatcher.Execute(() =>
                        {
                            int maxTextureSize = SystemInfo.maxTextureSize;
                            int padding = 8;

                            int x = 0;
                            int y = 0;
                            int width = 1;
                            int height = 1;

                            string[] textureNames = new string[textures.Length];
                            foreach (var item in types.Value)
                            {
                                x += item.Value.width + 8;
                                if (x > maxTextureSize)
                                {
                                    x = 0;
                                    y += item.Value.height + 8;
                                }

                                width = width.ClosestPowerOfTwo();
                                height = height.ClosestPowerOfTwo();
                            }

                            background = new Texture2D(maxTextureSize, maxTextureSize, TextureFormat.RGBA32, textureMetaData.generateMipmap);
                            rects = background.PackTextures(textures, padding, maxTextureSize);

                            background.filterMode = textureMetaData.filterMode;
                            background.mipMapBias = -0.4f;

                            if (textureMetaData.compressionType != TextureCompressionQuality.none)
                                background.Compress(textureMetaData.compressionType == TextureCompressionQuality.highQuality);
                        });

                        if (!Kernel.isPlaying)
                            return;

                        ThreadDispatcher.Execute(() =>
                        {
                            for (int i = 0; i < textures.Length; i++)
                                Object.Destroy(textures[i]);
                        }).Forget();

                        if (background == null || rects == null)
                            continue;

                        Dictionary<string, Rect> fileName_rect = new Dictionary<string, Rect>();
                        for (int i = 0; i < rects.Length; i++)
                            fileName_rect.Add(textureNames[i], rects[i]);

                        /* packTextureRects */ type_name_rect.Add(types.Key, fileName_rect);
                        /* packTextures */ type_texture.Add(types.Key, background);
                    }

                    /* packTextureRects */
                    tempPackTextureRects.Add(nameSpaces.Key, type_name_rect);
                    /* packTextures */
                    tempPackTextures.Add(nameSpaces.Key, type_texture);
                }
            }

            async UniTask LoadSprite()
            {
                List<UniTask> tasks = new List<UniTask>();

                foreach (var nameSpace in tempPackTextureRects)
                {
                    foreach (var type in nameSpace.Value)
                    {
                        foreach (var rects in type.Value)
                        {
                            if (!Kernel.isPlaying)
                                return;

                            tasks.Add(Task());

                            async UniTask Task()
                            {
                                Texture2D? background = SearchPackTexture(type.Key, nameSpace.Key);
                                if (background == null)
                                    return;

                                Rect rect = rects.Value;
                                rect = new Rect(rect.x * background.width, rect.y * background.height, rect.width * background.width, rect.height * background.height);

                                Dictionary<string, SpriteMetaData[]> spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(SearchTexturePath(type.Key, rects.Key, nameSpace.Key) + ".json") ?? new Dictionary<string, SpriteMetaData[]>(); ;
                                if (!spriteMetaDatas.ContainsKey(spriteDefaultTag))
                                    spriteMetaDatas.Add(spriteDefaultTag, new SpriteMetaData[] { new SpriteMetaData() });

                                foreach (var item in spriteMetaDatas)
                                {
                                    for (int i = 0; i < item.Value.Length; i++)
                                    {
                                        SpriteMetaData spriteMetaData = item.Value[i];
                                        if (spriteMetaData == null)
                                        {
                                            spriteMetaData = new SpriteMetaData();
                                            spriteMetaData.RectMinMax(rect.width, rect.height);
                                            spriteMetaDatas[item.Key][i] = spriteMetaData;
                                        }

                                        spriteMetaData.rect = new JRect(rect.x + spriteMetaData.rect.x, rect.y + spriteMetaData.rect.y, rect.width - (rect.width - spriteMetaData.rect.width), rect.height - (rect.height - spriteMetaData.rect.height));
                                    }
                                }

                                Dictionary<string, Sprite[]> sprites = await ThreadDispatcher.Execute(() => GetSprites(background, HideFlags.DontSave, spriteMetaDatas));

                                tempAllTextureSprites.TryAdd(nameSpace.Key, new Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>());
                                tempAllTextureSprites[nameSpace.Key].TryAdd(type.Key, new Dictionary<string, Dictionary<string, Sprite[]>>());
                                tempAllTextureSprites[nameSpace.Key][type.Key].TryAdd(rects.Key, sprites);
                            }
                        }
                    }
                }

                await UniTask.WhenAll(tasks);
                if (!Kernel.isPlaying)
                    return;
            }
        }
    }
}
