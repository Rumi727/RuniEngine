#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Jsons;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using Object = UnityEngine.Object;

namespace RuniEngine.Resource.Images
{
    public sealed class ImageLoader : IResourceElement
    {
        public bool isLoaded { get; private set; } = false;
        public ResourcePack? resourcePack { get; set; } = null;



        /// <summary>
        /// <see cref="Texture2D"/>? = allTextures[nameSpace][type];
        /// </summary>
        Dictionary<string, Dictionary<string, Texture2D?>> packTextures = new();
        /// <summary>
        /// <see cref="Rect"/> = allTextureRects[nameSpace][type][fileName];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, Rect>>> packTextureRects = new();
        /// <summary>
        /// <see cref="TextureMetaData"/> = packTextureMetaDatas[nameSpace][type];
        /// </summary>
        Dictionary<string, Dictionary<string, TextureMetaData>> textureMetaDatas = new();
        /// <summary>
        ///     <see cref="Dictionary{TKey, TValue}">
        ///         Dictionary&lt;<see cref="string"/>, <see cref="SpriteMetaData"/><see cref="Array">[]</see>&gt;
        ///     </see> = spriteMetaDatas[nameSpace][type][fileName];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, SpriteMetaData[]>>>> spriteMetaDatas = new();
        /// <summary>
        /// <see cref="Sprite"/>?<see cref="Array">[]</see> = allTextureSprites[nameSpace][type][fileName][tag];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>> allTextureSprites = new();



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
        public static Texture2D? GetTexture(IOHandler ioHandler, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(ioHandler.AddExtension(".json")) ?? new TextureMetaData();
            return GetTexture(ioHandler, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);
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
        public static Texture2D? GetTexture(IOHandler ioHandler, TextureMetaData textureMetaData, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave) => GetTexture(ioHandler, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);

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
        public static Texture2D? GetTexture(IOHandler ioHandler, FilterMode filterMode, bool generateMipmap, TextureCompressionQuality compressionType, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            if (ioHandler.FileExists())
            {
                Texture2D texture = new Texture2D(1, 1, textureFormat, generateMipmap, false);
                ResourceManager.RegisterManagedResource(texture);

                texture.filterMode = filterMode;
                texture.name = PathUtility.GetFileNameWithoutExtension(ioHandler.childPath);
                texture.hideFlags = hideFlags;
                texture.mipMapBias = -0.5f;

                AsyncImageLoader.LoaderSettings loaderSettings = AsyncImageLoader.LoaderSettings.Default;
                loaderSettings.generateMipmap = generateMipmap;
                loaderSettings.logException = true;

                if (!AsyncImageLoader.LoadImage(texture, ioHandler.ReadAllBytes(), loaderSettings))
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
        /// Asynchronously import an image file as a Texture2D type.
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
        public static UniTask<Texture2D?> GetTextureAsync(IOHandler ioHandler, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(ioHandler.AddExtension(".json")) ?? new TextureMetaData();
            return GetTextureAsync(ioHandler, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);
        }

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 비동기로 가져옵니다
        /// Asynchronously import an image file as a Texture2D type.
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
        public static UniTask<Texture2D?> GetTextureAsync(IOHandler ioHandler, TextureMetaData textureMetaData, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave) => GetTextureAsync(ioHandler, textureMetaData.filterMode, textureMetaData.generateMipmap, textureMetaData.compressionType, textureFormat, hideFlags);

        /// <summary>
        /// 이미지 파일을 Texture2D 타입으로 비동기로 가져옵니다
        /// Asynchronously import an image file as a Texture2D type.
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
        public static async UniTask<Texture2D?> GetTextureAsync(IOHandler ioHandler, FilterMode filterMode, bool generateMipmap, TextureCompressionQuality compressionType, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            if (ioHandler.FileExists())
            {
                byte[] textureBytes = await ioHandler.ReadAllBytesAsync();

                Texture2D texture = new Texture2D(1, 1, textureFormat, generateMipmap);
                ResourceManager.RegisterManagedResource(texture);

                texture.filterMode = filterMode;
                texture.name = PathUtility.GetFileNameWithoutExtension(ioHandler.childPath);
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

                ResourceManager.RegisterManagedResource(texture);

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
        /// 이미지 파일을 스프라이트로 가져옵니다 (Unity API를 사용하기 때문에 메인 스레드에서 실행해야 합니다.)
        /// Import image files as sprites (Since the Unity API is used, we need to run it on the main thread)
        /// </summary>
        /// <param name="resourcePackPath">
        /// 리소스팩 경로
        /// Resource Pack Path
        /// </param>
        /// <param name="type">
        /// 타입
        /// Type
        /// </param>
        /// <param name="name">
        /// 이름
        /// Name
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// Name Space
        /// </param>
        /// <param name="textureFormat">
        /// 텍스쳐 포맷
        /// Texture Format
        /// </param>
        /// <returns></returns>
        public static Sprite?[]? GetSprites(ResourcePack resourcePack, string type, string name, string nameSpace = "", string tag = spriteDefaultTag, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string rootPath = PathUtility.Combine(nameSpace, ImageLoader.name);
            string path = PathUtility.Combine(rootPath, type, name);

            resourcePack.ioHandler.CreateChild(path).FileExists(out IOHandler imageHandler, ExtensionFilter.pictureFileFilter);

            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData>(PathUtility.Combine(rootPath, type) + ".json");
            Texture2D? texture = GetTexture(imageHandler, textureMetaData, textureFormat);
            if (texture == null)
                return null;

            Dictionary<string, SpriteMetaData[]>? spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(imageHandler.AddExtension(".json"));
            if (spriteMetaDatas == null)
            {
                Object.DestroyImmediate(texture);
                return null;
            }

            ResourceManager.RegisterManagedResource(texture);

            Dictionary<string, Sprite?[]> sprites = GetSprites(texture, hideFlags, spriteMetaDatas);
            if (sprites.ContainsKey(tag))
                return sprites[tag];
            else if (sprites.ContainsKey(spriteDefaultTag))
                return sprites[spriteDefaultTag];

            return null;
        }

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

                ResourceManager.RegisterManagedResource(sprite);

                return sprite;
            }
            else
            {
                SpriteMetaData spriteMetaData2 = (SpriteMetaData)spriteMetaData;
                spriteMetaData2.RectMinMax(texture.width, texture.height);

                Sprite sprite = Sprite.Create(texture, spriteMetaData2.rect, spriteMetaData2.pivot, spriteMetaData2.pixelsPerUnit, 0, SpriteMeshType.FullRect, spriteMetaData2.border);
                sprite.name = texture.name;
                sprite.hideFlags = hideFlags;

                ResourceManager.RegisterManagedResource(sprite);

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
        public static Dictionary<string, Sprite?[]>? GetSprites(IOHandler ioHandler, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();

            Texture2D? texture = GetTexture(ioHandler, textureFormat);
            if (texture == null)
                return null;

            Dictionary<string, SpriteMetaData[]>? spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(ioHandler.AddExtension(".json"));
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
        public static Dictionary<string, Sprite?[]> GetSprites(Texture2D texture, HideFlags hideFlags, Dictionary<string, SpriteMetaData[]> spriteMetaDatas, string? name = null)
        {
            NotMainThreadException.Exception();

            if (!spriteMetaDatas.ContainsKey(spriteDefaultTag))
                spriteMetaDatas.Add(spriteDefaultTag, new SpriteMetaData[] { new SpriteMetaData() });

            Dictionary<string, Sprite?[]> sprites = new Dictionary<string, Sprite?[]>();
            foreach (var item in spriteMetaDatas)
            {
                SpriteMetaData[] spriteMetaDatas2 = item.Value;
                sprites.Add(item.Key, new Sprite[spriteMetaDatas2.Length]);

                for (int i = 0; i < spriteMetaDatas2.Length; i++)
                {
                    SpriteMetaData spriteMetaData = spriteMetaDatas2[i];
                    spriteMetaData.RectMinMax(texture.width, texture.height);

                    Sprite sprite = Sprite.Create(texture, spriteMetaData.rect, spriteMetaData.pivot, spriteMetaData.pixelsPerUnit, 0, SpriteMeshType.FullRect, spriteMetaData.border);
                    if (string.IsNullOrEmpty(name))
                        sprite.name = texture.name + " " + item.Key;
                    else
                        sprite.name = name + " " + item.Key;

                    sprite.hideFlags = hideFlags;

                    ResourceManager.RegisterManagedResource(sprite);
                    sprites[item.Key][i] = sprite;
                }
            }
            return sprites;
        }
        #endregion



        public static string[] GetTypes(ResourcePack resourcePack, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTextures.TryGetValue(nameSpace, out var value))
                {
                    result = value.Keys.ToArray();
                    return true;
                }

                return false;
            }))
            {
                string textureRootPath = PathUtility.Combine(nameSpace, name);
                IOHandler rootHandler = resourcePack.ioHandler.CreateChild(textureRootPath);
                if (!rootHandler.DirectoryExists())
                    return result;

                result = rootHandler.GetAllDirectories().ToArray();
                result = result.Insert(0, string.Empty);
            }

            return result;
        }

        public static string[] GetSpriteNames(ResourcePack resourcePack, string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                if (x.packTextureRects.TryGetValue(nameSpace, out var value) && value.TryGetValue(type, out var value2))
                {
                    result = value2.Keys.ToArray();
                    return true;
                }

                return false;
            }))
            {
                string typePath = PathUtility.Combine(nameSpace, name, type);
                IOHandler typeHandler = resourcePack.ioHandler.CreateChild(typePath);

                if (!typeHandler.DirectoryExists())
                    return result;

                result = typeHandler.GetFiles(ExtensionFilter.pictureFileFilter).Select(x => PathUtility.GetFileNameWithoutExtension(x)).ToArray();
            }

            return result;
        }



        public static TextureMetaData? SearchTextureMetaData(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            TextureMetaData? result = null;
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.textureMetaDatas.ContainsKey(nameSpace) && x.textureMetaDatas[nameSpace].ContainsKey(type))
                {
                    result = x.textureMetaDatas[nameSpace][type];
                    return true;
                }

                return false;
            });

            return result;
        }

        public static Dictionary<string, SpriteMetaData[]>? SearchSpriteMetaData(string type, string name, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            Dictionary<string, SpriteMetaData[]>? result = null;
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.spriteMetaDatas.ContainsKey(nameSpace) && x.spriteMetaDatas[nameSpace].ContainsKey(type) && x.spriteMetaDatas[nameSpace][type].ContainsKey(name))
                {
                    result = x.spriteMetaDatas[nameSpace][type][name];
                    return true;
                }

                return false;
            });

            return result;
        }

        public static Texture2D? SearchPackTexture(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            Texture2D? result = null;
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTextures.ContainsKey(nameSpace) && x.packTextures[nameSpace].ContainsKey(type))
                {
                    result = x.packTextures[nameSpace][type];
                    return true;
                }

                return false;
            });

            return result;
        }

        public static Rect SearchTextureRect(string type, string name, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            Rect result = Rect.zero;
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTextureRects.ContainsKey(nameSpace) && x.packTextureRects[nameSpace].ContainsKey(type) && x.packTextureRects[nameSpace][type].ContainsKey(name))
                {
                    result = x.packTextureRects[nameSpace][type][name];
                    return true;
                }

                return false;
            });

            return result;
        }

        public static Sprite?[]? SearchSprites(string type, string name, string nameSpace = "", string tag = spriteDefaultTag)
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            Sprite?[]? result = null;
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.allTextureSprites.ContainsKey(nameSpace) && x.allTextureSprites[nameSpace].ContainsKey(type) && x.allTextureSprites[nameSpace][type].ContainsKey(name))
                {
                    if (x.allTextureSprites[nameSpace][type][name].ContainsKey(tag))
                    {
                        result = x.allTextureSprites[nameSpace][type][name][tag];
                        return true;
                    }
                    else if (x.allTextureSprites[nameSpace][type][name].ContainsKey(spriteDefaultTag))
                    {
                        result = x.allTextureSprites[nameSpace][type][name][spriteDefaultTag];
                        return true;
                    }
                }

                return false;
            });

            return result;
        }




        public UniTask Load() => Load(null);
        public async UniTask Load(IProgress<float>? progress)
        {
            if (resourcePack == null)
                return;

            await UniTask.SwitchToThreadPool();

            foreach (Texture2D? texture in packTextures.SelectMany(item => item.Value)
                                                       .Select(x => x.Value))
                ResourceManager.RegisterGarbageResource(texture);

            foreach (Sprite? sprite in allTextureSprites.SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value))
                ResourceManager.RegisterGarbageResource(sprite);

            /// <summary>
            /// Texture2D = tempTextures[nameSpace][type][name];
            /// </summary>
            //Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempPackTexturePaths = new();
            //Dictionary<string, Dictionary<string, string>> tempPackTextureTypePaths = new();
            Dictionary<string, Dictionary<string, TextureMetaData>> tempTextureMetaDatas = new();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, SpriteMetaData[]>>>> tempSpriteMetaDatas = new();
            Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>> tempTextures = new();

            //Find Textures
            {
                int progressValue = 0;
                int maxProgress = 0;
                List<UniTask> tasks = new List<UniTask>();

                for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
                {
                    string nameSpace = resourcePack.nameSpaces[i];
                    string rootPath = PathUtility.Combine(nameSpace, name);
                    IOHandler root = resourcePack.ioHandler.CreateChild(rootPath);

                    if (!root.DirectoryExists())
                    {
                        progressValue++;
                        maxProgress++;

                        progress?.Report((float)progressValue / maxProgress);
                        continue;
                    }

                    GetImage(root.CreateChild(string.Empty));

                    foreach (var typePath in root.GetAllDirectories())
                        GetImage(root.CreateChild(typePath));

                    void GetImage(IOHandler handler)
                    {
                        TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(handler.AddExtension(".json")) ?? new TextureMetaData();
                        string typeName = handler.childPath;

                        tempTextureMetaDatas.TryAdd(nameSpace, new());
                        tempTextureMetaDatas[nameSpace].TryAdd(typeName, textureMetaData);

                        foreach (string filePath in handler.GetFiles(ExtensionFilter.pictureFileFilter))
                        {
                            IOHandler fileHandler = handler.CreateChild(filePath);
                            string fileName = PathUtility.GetFileNameWithoutExtension(filePath);

                            maxProgress++;
                            tasks.Add(Task());

                            //병렬 로드
                            async UniTask Task()
                            {
                                try
                                {
                                    Dictionary<string, SpriteMetaData[]> spriteMetaData = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>?>(fileHandler.AddExtension(".json")) ?? new();
                                    Texture2D? texture = await await ThreadDispatcher.Execute(() => GetTextureAsync(fileHandler, textureMetaData));
                                    if (texture == null)
                                        return;

                                    tempTextures.TryAdd(nameSpace, new());
                                    tempTextures[nameSpace].TryAdd(typeName, new());
                                    tempTextures[nameSpace][typeName].TryAdd(fileName, texture);

                                    tempSpriteMetaDatas.TryAdd(nameSpace, new());
                                    tempSpriteMetaDatas[nameSpace].TryAdd(typeName, new());
                                    tempSpriteMetaDatas[nameSpace][typeName].TryAdd(fileName, spriteMetaData);
                                }
                                catch (Exception e)
                                {
                                    ResourceManager.ExceptionLog(e, filePath, nameof(ImageLoader));
                                }
                                finally
                                {
                                    progressValue++;
                                    progress?.Report((float)progressValue / maxProgress);
                                }
                            }
                        }
                    }
                }

                await UniTask.WhenAll(tasks);
            }

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            textureMetaDatas = tempTextureMetaDatas;
            spriteMetaDatas = tempSpriteMetaDatas;

            await UniTask.SwitchToThreadPool();

            Dictionary<string, Dictionary<string, Dictionary<string, Rect>>> tempPackTextureRects = new();
            Dictionary<string, Dictionary<string, Texture2D?>> tempPackTextures = new();

            //Pack Textures
            {
                int progressValue = 0;
                int maxProgress = tempTextures.SelectMany(x => x.Value).Count();

                foreach (var nameSpaces in tempTextures)
                {
                    /* packTextureRects */
                    Dictionary<string, Dictionary<string, Rect>> type_name_rect = new Dictionary<string, Dictionary<string, Rect>>();
                    /* packTextures */
                    Dictionary<string, Texture2D?> type_texture = new Dictionary<string, Texture2D?>();

                    foreach (var types in nameSpaces.Value)
                    {
                        string[] textureNames = types.Value.Keys.ToArray();
                        Texture2D[] textures = types.Value.Values.ToArray();
                        Texture2D? background = null;
                        Rect[]? rects = null;

                        TextureMetaData textureMetaData = tempTextureMetaDatas[nameSpaces.Key][types.Key];
                        await ThreadDispatcher.Execute(() =>
                        {
                            int maxTextureSize = SystemInfo.maxTextureSize;
                            int padding = 8;

                            background = new Texture2D(1, 1, TextureFormat.RGBA32, textureMetaData.generateMipmap);
                            rects = background.PackTextures(textures, padding, maxTextureSize);

                            background.name = types.Key;

                            background.filterMode = textureMetaData.filterMode;
                            background.mipMapBias = -0.4f;

                            if (textureMetaData.compressionType != TextureCompressionQuality.none)
                                background.Compress(textureMetaData.compressionType == TextureCompressionQuality.highQuality);

                            background.hideFlags = HideFlags.DontSave;
                            ResourceManager.RegisterManagedResource(background);
                        });

                        ThreadDispatcher.ExecuteForget(() =>
                        {
                            for (int i = 0; i < textures.Length; i++)
                            {
                                Texture2D? texture = textures[i];
                                ResourceManager.UnregisterManagedResource(texture);

                                if (texture != null)
                                    Object.DestroyImmediate(texture);
                            }
                        });

                        if (background == null || rects == null)
                            continue;

                        Dictionary<string, Rect> fileName_rect = new Dictionary<string, Rect>();
                        for (int i = 0; i < rects.Length; i++)
                            fileName_rect.Add(textureNames[i], rects[i]);

                        /* packTextureRects */
                        type_name_rect.Add(types.Key, fileName_rect);
                        /* packTextures */
                        type_texture.Add(types.Key, background);

                        progressValue++;
                        progress?.Report((float)progressValue / maxProgress);
                    }

                    /* packTextureRects */
                    tempPackTextureRects.Add(nameSpaces.Key, type_name_rect);
                    /* packTextures */
                    tempPackTextures.Add(nameSpaces.Key, type_texture);
                }
            }

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            packTextureRects = tempPackTextureRects;
            packTextures = tempPackTextures;

            await UniTask.SwitchToThreadPool();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>> tempAllTextureSprites = new();

            // Create Sprites
            {
                int progressValue = 0;
                int maxProgress = 0;

                List<UniTask> tasks = new List<UniTask>();
                foreach
                (
                    var (nameSpace, type, rects) in tempPackTextureRects.SelectMany
                    (
                        nameSpaces => nameSpaces.Value.SelectMany
                        (
                            types => types.Value.Select
                            (
                                rects => (nameSpaces, types, rects)
                            )
                        )
                    )
                )
                {
                    maxProgress++;

                    tasks.Add(Task());
                    async UniTask Task()
                    {
                        Texture2D? background = SearchPackTexture(type.Key, nameSpace.Key);
                        if (background == null)
                            return;

                        Rect rect = rects.Value;
#if UNITY_6000_1_ORNEWER
                        rect = new Rect(rect.x * background.width, rect.y * background.height, rect.width * background.width, rect.height * background.height);
#else
                        await ThreadDispatcher.Execute(() => rect = new Rect(rect.x * background.width, rect.y * background.height, rect.width * background.width, rect.height * background.height));
#endif

                        Dictionary<string, SpriteMetaData[]> spriteMetaDatas = tempSpriteMetaDatas[nameSpace.Key][type.Key][rects.Key];
                        if (!spriteMetaDatas.ContainsKey(spriteDefaultTag))
                            spriteMetaDatas.Add(spriteDefaultTag, new SpriteMetaData[] { new SpriteMetaData() });

                        foreach (var item in spriteMetaDatas)
                        {
                            for (int i = 0; i < item.Value.Length; i++)
                            {
                                SpriteMetaData spriteMetaData = item.Value[i];

                                spriteMetaData.RectMinMax(rect.width, rect.height);
                                spriteMetaData.rect = new JRect(rect.x + spriteMetaData.rect.x, rect.y + spriteMetaData.rect.y, rect.width - (rect.width - spriteMetaData.rect.width), rect.height - (rect.height - spriteMetaData.rect.height));

                                spriteMetaDatas[item.Key][i] = spriteMetaData;
                            }
                        }

                        Dictionary<string, Sprite?[]> sprites = await ThreadDispatcher.Execute(() => GetSprites(background, HideFlags.DontSave, spriteMetaDatas, rects.Key));

                        tempAllTextureSprites.TryAdd(nameSpace.Key, new Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>());
                        tempAllTextureSprites[nameSpace.Key].TryAdd(type.Key, new Dictionary<string, Dictionary<string, Sprite?[]>>());
                        tempAllTextureSprites[nameSpace.Key][type.Key].TryAdd(rects.Key, sprites);

                        progressValue++;
                        progress?.Report((float)progressValue / maxProgress);
                    }
                }

                await UniTask.WhenAll(tasks);
            }

            progress?.Report(1);

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);
            allTextureSprites = tempAllTextureSprites;

            isLoaded = true;
        }

        public async UniTask Unload()
        {
            isLoaded = false;

            foreach (Sprite? item in allTextureSprites.SelectMany(x => x.Value)
                                                      .SelectMany(x => x.Value)
                                                      .SelectMany(x => x.Value)
                                                      .SelectMany(x => x.Value)
                                                      .Where(x => x != null))
            {
                ResourceManager.UnregisterManagedResource(item);

                Object.DestroyImmediate(item);
                await UniTask.Yield();
            }

            foreach (Texture2D? texture in packTextures.SelectMany(x => x.Value)
                                                       .Select(x => x.Value)
                                                       .Where(x => x != null))
            {
                ResourceManager.UnregisterManagedResource(texture);

                Object.DestroyImmediate(texture);
                await UniTask.Yield();
            }

            packTextures = new();
            packTextureRects = new();
            spriteMetaDatas = new();
            textureMetaDatas = new();
            allTextureSprites = new();
        }
    }
}
