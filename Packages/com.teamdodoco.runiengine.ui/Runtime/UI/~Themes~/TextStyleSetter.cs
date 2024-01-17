#nullable enable
using System;
using TMPro;
using UnityEngine;

namespace RuniEngine.UI.Themes
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TextStyleSetter : StyleSetterBase
    {
        public TMP_Text? text => _text;
        [SerializeField, NotNullField] TMP_Text? _text;

        public override void Refresh(Type? editInScriptType, ThemeStyle style)
        {
            if (text == null)
                return;

            editInScript = editInScriptType;

            text.fontSize = style.text.fontSize;

            text.enableAutoSizing = style.text.autoSizing.enableAutoSizing;

            text.fontSizeMin = style.text.autoSizing.fontSizeMin;
            text.fontSizeMax = style.text.autoSizing.fontSizeMax;

            text.characterWidthAdjustment = style.text.autoSizing.characterWidthAdjustment;
            text.lineSpacingAdjustment = style.text.autoSizing.lineSpacingAdjustment;

            text.color = style.text.color;

            text.characterSpacing = style.text.spacing.character;
            text.wordSpacing = style.text.spacing.word;
            text.lineSpacing = style.text.spacing.line;
            text.paragraphSpacing = style.text.spacing.paragraph;

            text.alignment = style.text.alignment;

            text.margin = new Vector4(style.text.padding.left, style.text.padding.right, style.text.padding.top, style.text.padding.bottom);
        }
    }
}
