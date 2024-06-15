#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Threading;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using RuniEngine.Jsons;
using System.Collections.Generic;
using System.Linq;
using System;

using Object = UnityEngine.Object;

namespace RuniEngine.Resource.Images
{
    public sealed class ImageLoader : IResourceElement
    {
        public bool isLoaded { get; private set; } = false;
        public ResourcePack? resourcePack { get; set; } = null;



        /// <summary>
        /// Texture2D = allTextures[nameSpace][type];
        /// </summary>
        Dictionary<string, Dictionary<string, Texture2D?>> packTextures = new();
        /// <summary>
        /// Rect = allTextureRects[nameSpace][type][fileName];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, Rect>>> packTextureRects = new();
        /// <summary>
        /// string = allTexturePaths[nameSpace][type][fileName];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> packTexturePaths = new();
        /// <summary>
        /// string = allTexturePaths[nameSpace][type];
        /// </summary>
        Dictionary<string, Dictionary<string, string>> packTextureTypePaths = new();
        /// <summary>
        /// Sprite = allTextureSprites[nameSpace][type][fileName][tag];
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
                Texture2D texture = new Texture2D(1, 1, textureFormat, generateMipmap, false);
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
        public static Sprite?[]? GetSprites(string type, string name, string nameSpace = "", string tag = spriteDefaultTag, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
        {
            NotMainThreadException.Exception();
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string rootPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, ImageLoader.name);
            string path = Path.Combine(rootPath, type, name);
            
            ResourceManager.FileExtensionExists(path, out path, ExtensionFilter.pictureFileFilter);
            TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData>(Path.Combine(rootPath, type) + ".json");
            Texture2D? texture = GetTexture(path, textureMetaData, textureFormat);
            if (texture == null)
                return null;
            
            Dictionary<string, SpriteMetaData[]>? spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(path + ".json");
            if (spriteMetaDatas == null)
            {
                Object.DestroyImmediate(texture);
                return null;
            }

            ResourceManager.allLoadedResources.Add(texture);

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

                ResourceManager.allLoadedResources.Add(sprite);

                return sprite;
            }
            else
            {
                SpriteMetaData spriteMetaData2 = (SpriteMetaData)spriteMetaData;
                spriteMetaData2.RectMinMax(texture.width, texture.height);

                Sprite sprite = Sprite.Create(texture, spriteMetaData2.rect, spriteMetaData2.pivot, spriteMetaData2.pixelsPerUnit, 0, SpriteMeshType.FullRect, spriteMetaData2.border);
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
        public static Dictionary<string, Sprite?[]>? GetSprites(string path, TextureFormat textureFormat = TextureFormat.RGBA32, HideFlags hideFlags = HideFlags.DontSave)
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
        public static Dictionary<string, Sprite?[]> GetSprites(Texture2D texture, HideFlags hideFlags, Dictionary<string, SpriteMetaData[]> spriteMetaDatas)
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
                    sprite.name = texture.name;
                    sprite.hideFlags = hideFlags;

                    ResourceManager.allLoadedResources.Add(sprite);
                    sprites[item.Key][i] = sprite;
                }
            }
            return sprites;
        }
        #endregion



        public static string[] GetTypes(string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result2 = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTextures.TryGetValue(nameSpace, out var result))
                {
                    result2 = result.Keys.ToArray();
                    return true;
                }

                return false;
            }))
            {
                string textureRootPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                if (!Directory.Exists(textureRootPath))
                    return result2;

                string[] typePaths = Directory.GetDirectories(textureRootPath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < typePaths.Length; i++)
                {
                    string typePath = typePaths[i];
                    typePaths[i] = typePath.Substring(textureRootPath.Length + 1, typePath.Length - textureRootPath.Length - 1).Replace("\\", "/");
                }

                //빈 타입 경로 만들기
                string[] typePaths2 = new string[typePaths.Length + 1];
                Array.Copy(typePaths, 0, typePaths2, 1, typePaths.Length);

                result2 = typePaths2;
            }

            return result2;
        }

        public static string[] GetSpriteNames(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result3 = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                if (x.packTextureRects.TryGetValue(nameSpace, out var result) && result.TryGetValue(type, out var result2))
                {
                    result3 = result2.Keys.ToArray();
                    return true;
                }

                return false;
            }))
            {
                string typePath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name, type);
                if (!Directory.Exists(typePath))
                    return result3;

                string[] paths = DirectoryUtility.GetFiles(typePath, ExtensionFilter.pictureFileFilter);
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = Path.GetFileNameWithoutExtension(paths[i]).Replace("\\", "/");

                result3 = paths;
            }

            return result3;
        }



        public static string SearchTexturePath(string type, string name, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string result = "";
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTexturePaths.ContainsKey(nameSpace) && x.packTexturePaths[nameSpace].ContainsKey(type) && x.packTexturePaths[nameSpace][type].ContainsKey(name))
                {
                    result = x.packTexturePaths[nameSpace][type][name];
                    return true;
                }

                return false;
            });

            return result;
        }

        public static string SearchTextureTypePath(string type, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string result = "";
            ResourceManager.ResourceElementLoop<ImageLoader>(x =>
            {
                if (x.packTextureTypePaths.ContainsKey(nameSpace) && x.packTextureTypePaths[nameSpace].ContainsKey(type))
                {
                    result = x.packTextureTypePaths[nameSpace][type];
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
                ResourceManager.garbages.Add(texture);

            foreach (Sprite? sprite in allTextureSprites.SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value)
                                                        .SelectMany(x => x.Value))
                ResourceManager.garbages.Add(sprite);

            /// <summary>
            /// Texture2D = tempTextures[nameSpace][type][name];
            /// </summary>
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempPackTexturePaths = new();
            Dictionary<string, Dictionary<string, string>> tempPackTextureTypePaths = new();
            Dictionary<string, Dictionary<string, Dictionary<string, Texture2D>>> tempTextures = new();

            //Find Textures
            {
                int progressValue = 0;
                int maxProgress = 0;
                List<UniTask> tasks = new List<UniTask>();

                for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
                {
                    string nameSpace = resourcePack.nameSpaces[i];
                    string rootPath = Path.Combine(resourcePack.path, ResourceManager.rootName, nameSpace, name);
                    if (!Directory.Exists(rootPath))
                    {
                        progressValue++;
                        maxProgress++;

                        progress?.Report((float)progressValue / maxProgress);
                        continue;
                    }

                    string[] typePaths = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

                    for (int j = -1; j < typePaths.Length; j++)
                    {
                        string typePath;
                        string typeName;

                        if (j >= 0)
                        {
                            typePath = typePaths[j].UniformDirectorySeparatorCharacter();
                            typeName = typePath.Substring(rootPath.Length + 1);
                        }
                        else
                        {
                            typePath = rootPath;
                            typeName = "";
                        }

                        string[] filePaths = DirectoryUtility.GetFiles(typePath, ExtensionFilter.pictureFileFilter);
                        for (int k = 0; k < filePaths.Length; k++)
                        {
                            string filePath = filePaths[k].UniformDirectorySeparatorCharacter();
                            string fileName = Path.GetFileNameWithoutExtension(filePath);

                            maxProgress++;
                            tasks.Add(Task());

                            //병렬 로드
                            async UniTask Task()
                            {
                                try
                                {
                                    TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(typePath + ".json") ?? new TextureMetaData();
                                    Texture2D? texture = await await ThreadDispatcher.Execute(() => GetTextureAsync(filePath, textureMetaData));
                                    if (texture == null)
                                        return;

                                    tempPackTexturePaths.TryAdd(nameSpace, new());
                                    tempPackTexturePaths[nameSpace].TryAdd(typeName, new());
                                    tempPackTexturePaths[nameSpace][typeName].TryAdd(fileName, filePath);

                                    tempPackTextureTypePaths.TryAdd(nameSpace, new());
                                    tempPackTextureTypePaths[nameSpace].TryAdd(typeName, typePath);

                                    tempTextures.TryAdd(nameSpace, new());
                                    tempTextures[nameSpace].TryAdd(typeName, new());
                                    tempTextures[nameSpace][typeName].TryAdd(fileName, texture);
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

            packTexturePaths = tempPackTexturePaths;
            packTextureTypePaths = tempPackTextureTypePaths;

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

                        TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData?>(tempPackTextureTypePaths[nameSpaces.Key][types.Key] + ".json") ?? new TextureMetaData();
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
                            ResourceManager.allLoadedResources.Add(background);
                        });

                        ThreadDispatcher.Execute(() =>
                        {
                            for (int i = 0; i < textures.Length; i++)
                                Object.DestroyImmediate(textures[i]);
                        }).Forget();

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
                int maxProgress = tempPackTextureRects.SelectMany(nameSpaces => nameSpaces.Value)
                                                      .SelectMany(types => types.Value)
                                                      .Count();

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
                    tasks.Add(Task());
                    async UniTask Task()
                    {
                        Texture2D? background = SearchPackTexture(type.Key, nameSpace.Key);
                        if (background == null)
                            return;

                        Rect rect = rects.Value;
                        rect = new Rect(rect.x * background.width, rect.y * background.height, rect.width * background.width, rect.height * background.height);

                        Dictionary<string, SpriteMetaData[]> spriteMetaDatas = JsonManager.JsonRead<Dictionary<string, SpriteMetaData[]>>(SearchTexturePath(type.Key, rects.Key, nameSpace.Key) + ".json") ?? new Dictionary<string, SpriteMetaData[]>();
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

                        Dictionary<string, Sprite?[]> sprites = await ThreadDispatcher.Execute(() => GetSprites(background, HideFlags.DontSave, spriteMetaDatas));

                        tempAllTextureSprites.TryAdd(nameSpace.Key, new Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>());
                        tempAllTextureSprites[nameSpace.Key].TryAdd(type.Key, new Dictionary<string, Dictionary<string, Sprite?[]>>());
                        tempAllTextureSprites[nameSpace.Key][type.Key].TryAdd(rects.Key, sprites);

                        progressValue++;
                        progress?.Report((float)progressValue / maxProgress);
                    }
                }

                await UniTask.WhenAll(tasks);
            }

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
                Object.DestroyImmediate(item);
                await UniTask.Yield();
            }

            foreach (Texture2D? texture in packTextures.SelectMany(x => x.Value)
                                                       .Select(x => x.Value)
                                                       .Where(x => x != null))
            {
                Object.DestroyImmediate(texture);
                await UniTask.Yield();
            }

            packTextures = new();
            packTextureRects = new();
            packTexturePaths = new();
            packTextureTypePaths = new();
            allTextureSprites = new();
        }
    }
}
