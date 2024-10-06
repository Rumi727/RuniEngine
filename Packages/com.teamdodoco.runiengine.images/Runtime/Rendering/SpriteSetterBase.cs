using RuniEngine.Booting;
using RuniEngine.Resource;
using RuniEngine.Resource.Images;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Rendering
{
    public abstract class SpriteSetterBase : MonoBehaviour, IRenderer
    {
        /// <summary>
        /// cachedLocalSprites[nameSpace][type][name][tag] = Sprite[];
        /// </summary>
        protected static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>> cachedLocalSprites = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite?[]>>>>();

        public Sprite? defaultSprite => _defaultSprite;
        [SerializeField] Sprite? _defaultSprite;

        public string nameSpace
        {
            get => _nameSpace; set
            {
                _nameSpace = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }
        [SerializeField] string _nameSpace = "";

        public string type
        {
            get => _type; set
            {
                _type = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }
        [SerializeField] string _type = "";

        public string spriteName
        {
            get => _path; set
            {
                _path = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }

        string IRenderer.path
        {
            get => _path; set
            {
                _path = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }
        [SerializeField] string _path = "";

        public string spriteTag
        {
            get => _spriteTag; set
            {
                _spriteTag = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }
        [SerializeField] string _spriteTag = "";

        public int index
        {
            get => _index; set
            {
                _index = value;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }
        [SerializeField, Min(0)] int _index = 0;

        public NameSpaceIndexTypeNamePair pair
        {
            get => new NameSpaceIndexTypeNamePair(nameSpace, index, type, spriteName);
            set
            {
                nameSpace = value.nameSpace;
                index = value.index;
                type = value.type;
                spriteName = value.name;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }

        NameSpacePathPair IRenderer.pair
        {
            get => new NameSpacePathPair(nameSpace, spriteName);
            set
            {
                nameSpace = value.nameSpace;
                spriteName = value.path;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }

        public Sprite? GetSprite()
        {
            Sprite? sprite = GetSprite(type, spriteName, index, nameSpace, spriteTag);
            if (sprite == null)
                return defaultSprite;

            return sprite;
        }

        public static Sprite? GetSprite(string type, string name, int index, string nameSpace = "", string tag = ImageLoader.spriteDefaultTag)
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            Sprite?[]? sprites;
            if (BootLoader.isAllLoaded)
                sprites = ImageLoader.SearchSprites(type, name, nameSpace, tag);
            else
            {
                if (cachedLocalSprites.ContainsKey(nameSpace) && cachedLocalSprites[nameSpace].ContainsKey(type) && cachedLocalSprites[nameSpace][type].ContainsKey(name) && cachedLocalSprites[nameSpace][type][name].ContainsKey(tag))
                {
                    sprites = cachedLocalSprites[nameSpace][type][name][tag];
                    if (sprites.Length > 0)
                    {
                        Sprite? sprite = sprites[index.Clamp(0, sprites.Length - 1)];
                        if (sprite != null)
                            return sprite;
                    }
                }

                sprites = ImageLoader.GetSprites(type, name, nameSpace, tag);
                if (sprites != null)
                {
                    cachedLocalSprites.TryAdd(nameSpace, new());
                    cachedLocalSprites[nameSpace].TryAdd(type, new());
                    cachedLocalSprites[nameSpace][type].TryAdd(name, new());
                    cachedLocalSprites[nameSpace][type][name][tag] = sprites;
                }
            }

            if (sprites != null && sprites.Length > 0)
                return sprites[index.Clamp(0, sprites.Length - 1)];

            return null;
        }

        public virtual void Awake() => Refresh();

        public abstract void Refresh();
    }
}
