/*#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI.Themes
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class LayoutGroupStyleSetter : StyleSetterBase
    {
        public HorizontalOrVerticalLayoutGroup? group => _group;
        [SerializeField, NotNullField] HorizontalOrVerticalLayoutGroup? _group;

        public override void Refresh(Type? editInScriptType, ThemeStyle style)
        {
            if (group == null)
                return;

            editInScript = editInScriptType;

            group.padding = (UnityEngine.RectOffset)style.layoutGroup.padding;
            group.spacing = style.layoutGroup.spacing;

            group.childAlignment = style.layoutGroup.childAlignment;

            group.reverseArrangement = style.layoutGroup.reverseArrangement;
        }
    }
}*/
