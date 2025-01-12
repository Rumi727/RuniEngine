#nullable enable
using RuniEngine.Resource;
using RuniEngine.Resource.Images;
using RuniEngine.UI.Themes;
using TMPro;
using UnityEditor;
using UnityEngine;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class ThemeStyleEditor : IThemeStyleEditor
    {
        public string label { get; } = "gui.theme";

        public virtual bool DrawGUI(ThemeStyle style)
        {
            bool changed = false;

            string nameSpaceLabel = TryGetText("gui.namespace");
            string nameLabel = TryGetText("gui.name");
            string colorLabel = TryGetText("gui.color");
            string indexLabel = TryGetText("gui.index");
            string blurLabel = TryGetText("texture_style.use_blur");
            string spacingLabel = TryGetText("gui.spacing");
            string paddingLabel = TryGetText("gui.padding");
            string alignmentLabel = TryGetText("gui.alignment");
            string fontSizeLabel = TryGetText("text_style.font_size");
            string autoSizingLabel = TryGetText("text_style.auto_sizing");
            string enableAutoSizingLabel = TryGetText("text_style.auto_sizing.enable_auto_sizing");
            string fontSizeMinLabel = TryGetText("text_style.auto_sizing.font_size_min");
            string fontSizeMaxLabel = TryGetText("text_style.auto_sizing.font_size_max");
            string characterWidthAdjustmentLabel = TryGetText("text_style.auto_sizing.character_width_adjustment");
            string lineSpacingAdjustmentLabel = TryGetText("text_style.auto_sizing.line_spacing_adjustment");
            string characterLabel = TryGetText("gui.character");
            string wordLabel = TryGetText("gui.word");
            string lineLabel = TryGetText("gui.line");
            string paragraphLabel = TryGetText("gui.paragraph");
            string reverseLabel = TryGetText("gui.reverse");
            string transitionLabel = TryGetText("gui.transition");
            string normalLabel = TryGetText("gui.normal");
            string highlightLabel = TryGetText("gui.highlight");
            string pressedLabel = TryGetText("gui.pressed");
            string selectLabel = TryGetText("gui.select");
            string disableLabel = TryGetText("gui.disable");
            string offsetLabel = TryGetText("rect_transform.offsets");
            string anchorMinLabel = TryGetText("rect_transform.anchor_min");
            string anchorMaxLabel = TryGetText("rect_transform.anchor_max");
            string pivotLabel = TryGetText("rect_transform.pivot");
            string typeLabel = TryGetText("gui.type");

            changed |= ThemeProjectSettings.DrawStyleGUI(TryGetText("gui.rect_transform"), "rectTransform", () =>
            {
                ThemeProjectSettings.BeginStyleFieldGUI();

                style.rectTransform.offset = RectOffsetField(offsetLabel, style.rectTransform.offset);

                Space();

                style.rectTransform.anchorMin = EditorGUILayout.Vector2Field(anchorMinLabel, style.rectTransform.anchorMin);
                style.rectTransform.anchorMax = EditorGUILayout.Vector2Field(anchorMaxLabel, style.rectTransform.anchorMax);

                Space();

                style.rectTransform.pivot = EditorGUILayout.Vector2Field(pivotLabel, style.rectTransform.pivot);

                ThemeProjectSettings.EndStyleFieldGUI();
            });

            changed |= ThemeProjectSettings.DrawStyleGUI(TryGetText("gui.texture"), "texture", () =>
            {
                ThemeProjectSettings.BeginStyleFieldGUI();

                style.texture.pair = NameSpaceIndexTypeNamePairField(style.texture.pair);

                Space();

                style.texture.color = EditorGUILayout.ColorField(colorLabel, style.texture.color);

                ThemeProjectSettings.EndStyleFieldGUI();
            });

            changed |= ThemeProjectSettings.DrawStyleGUI(TryGetText("gui.text"), "text", () =>
            {
                ThemeProjectSettings.BeginStyleFieldGUI();

                style.text.fontSize = EditorGUILayout.FloatField(fontSizeLabel, style.text.fontSize).Clamp(0, 32767);
                style.text.color = EditorGUILayout.ColorField(colorLabel, style.text.color);
                style.text.alignment = (TextAlignmentOptions)EditorGUILayout.EnumPopup(alignmentLabel, style.text.alignment);
                style.text.padding = RectOffsetField(paddingLabel, style.text.padding);

                ThemeProjectSettings.DrawStyleGUI(autoSizingLabel, "text.autoSizing", () =>
                {
                    ThemeProjectSettings.BeginStyleFieldGUI();

                    style.text.autoSizing.enableAutoSizing = EditorGUILayout.Toggle(enableAutoSizingLabel, style.text.autoSizing.enableAutoSizing);

                    if (style.text.autoSizing.enableAutoSizing)
                    {
                        Space();

                        style.text.autoSizing.fontSizeMin = EditorGUILayout.FloatField(fontSizeMinLabel, style.text.autoSizing.fontSizeMin).Clamp(0, style.text.autoSizing.fontSizeMax);
                        style.text.autoSizing.fontSizeMax = EditorGUILayout.FloatField(fontSizeMaxLabel, style.text.autoSizing.fontSizeMax).Clamp(style.text.autoSizing.fontSizeMin, 32767);

                        Space();

                        style.text.autoSizing.characterWidthAdjustment = EditorGUILayout.FloatField(characterWidthAdjustmentLabel, style.text.autoSizing.characterWidthAdjustment).Clamp(0, 50);
                        style.text.autoSizing.lineSpacingAdjustment = EditorGUILayout.FloatField(lineSpacingAdjustmentLabel, style.text.autoSizing.lineSpacingAdjustment).Clamp(float.MinValue, 0);
                    }

                    ThemeProjectSettings.EndStyleFieldGUI();
                });

                ThemeProjectSettings.DrawStyleGUI(spacingLabel, "texture.spacing", () =>
                {
                    ThemeProjectSettings.BeginStyleFieldGUI();

                    style.text.spacing.character = EditorGUILayout.FloatField(characterLabel, style.text.spacing.character);
                    style.text.spacing.word = EditorGUILayout.FloatField(wordLabel, style.text.spacing.word);
                    style.text.spacing.line = EditorGUILayout.FloatField(lineLabel, style.text.spacing.line);
                    style.text.spacing.paragraph = EditorGUILayout.FloatField(paragraphLabel, style.text.spacing.paragraph);

                    ThemeProjectSettings.EndStyleFieldGUI();
                });

                ThemeProjectSettings.EndStyleFieldGUI();
            });

            changed |= ThemeProjectSettings.DrawStyleGUI(TryGetText("gui.layout_group"), "layoutGroup", () =>
            {
                ThemeProjectSettings.BeginStyleFieldGUI();

                style.layoutGroup.spacing = EditorGUILayout.FloatField(spacingLabel, style.layoutGroup.spacing);
                style.layoutGroup.padding = RectOffsetField(paddingLabel, style.layoutGroup.padding);

                Space();

                style.layoutGroup.childAlignment = (TextAnchor)EditorGUILayout.EnumPopup(alignmentLabel, style.layoutGroup.childAlignment);
                style.layoutGroup.reverseArrangement = EditorGUILayout.Toggle(reverseLabel, style.layoutGroup.reverseArrangement);

                ThemeProjectSettings.EndStyleFieldGUI();
            });

            changed |= ThemeProjectSettings.DrawStyleGUI(TryGetText("gui.selectable"), "selectable", () =>
            {
                ThemeProjectSettings.BeginStyleFieldGUI();

                style.selectable.transition = (SelectableStyle.Transition)EditorGUILayout.EnumPopup(transitionLabel, style.selectable.transition);

                if (style.selectable.transition == SelectableStyle.Transition.ColorTint)
                {
                    Space();

                    style.selectable.colors.normalColor = EditorGUILayout.ColorField(normalLabel, style.selectable.colors.normalColor);
                    style.selectable.colors.highlightedColor = EditorGUILayout.ColorField(highlightLabel, style.selectable.colors.highlightedColor);
                    style.selectable.colors.pressedColor = EditorGUILayout.ColorField(pressedLabel, style.selectable.colors.pressedColor);
                    style.selectable.colors.selectedColor = EditorGUILayout.ColorField(selectLabel, style.selectable.colors.selectedColor);
                    style.selectable.colors.disabledColor = EditorGUILayout.ColorField(disableLabel, style.selectable.colors.disabledColor);
                }
                else if (style.selectable.transition == SelectableStyle.Transition.SpriteSwap)
                {

                    style.selectable.spriteState.highlightedSprite = DrawGUI(style.selectable.spriteState.highlightedSprite, highlightLabel, "selectable.spritetState.highlightedSprite");
                    style.selectable.spriteState.pressedSprite = DrawGUI(style.selectable.spriteState.pressedSprite, pressedLabel, "selectable.spriteState.pressedSprite");
                    style.selectable.spriteState.selectedSprite = DrawGUI(style.selectable.spriteState.selectedSprite, selectLabel, "selectable.spriteState.selectedSprite");
                    style.selectable.spriteState.disabledSprite = DrawGUI(style.selectable.spriteState.disabledSprite, disableLabel, "selectable.spriteState.disabledSprite");

                    NameSpaceIndexTypeNamePair DrawGUI(NameSpaceIndexTypeNamePair pair, string label, string foldKey)
                    {
                        ThemeProjectSettings.DrawStyleGUI(TryGetText(label), foldKey, () =>
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);

                            pair = NameSpaceIndexTypeNamePairField(pair);

                            GUILayout.EndVertical();
                        });

                        return pair;
                    }
                }

                ThemeProjectSettings.EndStyleFieldGUI();
            });

            return changed;
        }

        protected static NameSpaceIndexTypeNamePair NameSpaceIndexTypeNamePairField(NameSpaceIndexTypeNamePair value)
        {
            value.nameSpace = DrawNameSpace(ThemeProjectSettings.GetAdvancedDropdown(), TryGetText("gui.namespace"), value.nameSpace);
            value.type = DrawStringArray(ThemeProjectSettings.GetAdvancedDropdown(), TryGetText("gui.type"), value.type, ImageLoader.GetTypes(ResourcePack.defaultPack, value.nameSpace), true);
            value.name = DrawStringArray(ThemeProjectSettings.GetAdvancedDropdown(), TryGetText("gui.name"), value.name, ImageLoader.GetSpriteNames(ResourcePack.defaultPack, value.type, value.nameSpace));

            Space();

            value.index = EditorGUILayout.IntField(TryGetText("gui.index"), value.index).Clamp(0);
            return value;
        }
    }
}
