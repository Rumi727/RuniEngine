#nullable enable
using RuniEngine.Resource;
using RuniEngine.Resource.Images;
using UnityEngine;

namespace RuniEngine.Rendering
{
    public abstract class SpriteSetterBase : MonoBehaviour, IRenderer
    {
        public Sprite? defaultSprite => _defaultSprite;
        [SerializeField] Sprite? _defaultSprite;

        public string nameSpace { get => _nameSpace; set => _nameSpace = value; }
        [SerializeField] string _nameSpace = "";

        public string type { get => _type; set => _type = value; }
        [SerializeField] string _type = "";

        public string spriteName { get => _path; set => _path = value; }

        string IRenderer.path { get => _path; set => _path = value; }
        [SerializeField] string _path = "";

        public string spriteTag { get => _spriteTag; set => _spriteTag = value; }
        [SerializeField] string _spriteTag = "";

        public int index { get => _index; set => _index = value; }
        [SerializeField, Min(0)] int _index = 0;

        public NameSpaceIndexTypeNamePair pair
        {
            get => new NameSpaceIndexTypeNamePair(nameSpace, index, type, spriteName);
            set
            {
                nameSpace = value.nameSpace;
                spriteName = value.name;
            }
        }

        NameSpacePathPair IRenderer.pair
        {
            get => new NameSpacePathPair(nameSpace, spriteName);
            set
            {
                nameSpace = value.nameSpace;
                spriteName = value.path;
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

            Sprite[]? sprites = ImageLoader.SearchSprites(type, name, nameSpace, tag);
            if (sprites != null && sprites.Length > 0)
                return sprites[index.Clamp(0, sprites.Length - 1)];

            return null;
        }

        public abstract void Refresh();
    }
}
