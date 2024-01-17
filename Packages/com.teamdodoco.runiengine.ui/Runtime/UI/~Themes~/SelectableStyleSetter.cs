#nullable enable
using RuniEngine.Rendering;
using RuniEngine.Resource.Images;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI.Themes
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SelectableStyleSetter : StyleSetterBase
    {
        public Selectable? selectable => _selectable;
        [SerializeField, NotNullField] Selectable? _selectable;

        public override void Refresh(Type? editInScriptType, ThemeStyle style)
        {
            if (selectable == null)
                return;

            editInScript = editInScriptType;
            selectable.transition = (Selectable.Transition)style.selectable.transition;

            if (style.selectable.transition == SelectableStyle.Transition.ColorTint)
            {
                ColorBlock colorBlock = new ColorBlock()
                {
                    normalColor = style.selectable.colors.normalColor,
                    highlightedColor = style.selectable.colors.highlightedColor,
                    pressedColor = style.selectable.colors.pressedColor,
                    selectedColor = style.selectable.colors.selectedColor,
                    disabledColor = style.selectable.colors.disabledColor
                };

                selectable.colors = colorBlock;
            }
            else if (style.selectable.transition == SelectableStyle.Transition.SpriteSwap)
            {
                SpriteState spriteState = new SpriteState()
                {
                    highlightedSprite = GetSprite(style.selectable.spriteState.highlightedSprite),
                    pressedSprite = GetSprite(style.selectable.spriteState.pressedSprite),
                    selectedSprite = GetSprite(style.selectable.spriteState.selectedSprite),
                    disabledSprite = GetSprite(style.selectable.spriteState.disabledSprite)
                };

                selectable.spriteState = spriteState;
            }

            static Sprite? GetSprite(NameSpaceIndexTypeNamePair style) => SpriteSetterBase.GetSprite(style.type, style.name, style.index, style.nameSpace);
        }
    }
}
