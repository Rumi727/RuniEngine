#nullable enable
using RuniEngine.Rendering;
using RuniEngine.Resource;
using RuniEngine.Resource.Themes;
using System;
using UnityEngine;

namespace RuniEngine.UI.Themes
{
    public abstract class StyleSetterBase : UIBase, IRenderer
    {
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

        public string key
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



        public NameSpacePathPair pair
        {
            get => new NameSpacePathPair(nameSpace, key);
            set
            {
                nameSpace = value.nameSpace;
                key = value.path;

                if (isActiveAndEnabled)
                    Refresh();
            }
        }



        public Type? editInScript { get; set; }



        protected override void OnEnable() => Refresh();

        public virtual void Refresh()
        {
            if (editInScript != null)
                return;
            
            ThemeStyle? style = ThemeLoader.GetStyle(ResourcePack.defaultPack, key, nameSpace);
            if (style == null)
                return;

            Refresh(null, style);
        }

        public abstract void Refresh(Type? editInScript, ThemeStyle style);
    }
}
