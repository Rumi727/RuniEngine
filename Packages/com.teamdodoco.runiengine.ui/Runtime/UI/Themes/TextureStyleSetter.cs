#nullable enable
using RuniEngine.Rendering;
using System;
using UnityEngine;

namespace RuniEngine.UI.Themes
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TextureStyleSetter : StyleSetterBase
    {
        public ImageSetter? image => _image;
        [SerializeField, NotNullField] ImageSetter? _image;

        public override void Refresh(Type? editInScriptType, ThemeStyle style)
        {
            if (image == null || image.image == null)
                return;

            editInScript = editInScriptType;

            image.pair = style.texture.pair;
            image.image.color = style.texture.color;

            image.Refresh();
        }
    }
}
