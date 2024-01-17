#nullable enable
using RuniEngine.Editor.ProjectSettings;
using RuniEngine.Json;
using RuniEngine.Resource.Themes;
using RuniEngine.UI.Themes;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Themes
{
    public abstract class StyleSetterBaseEditor<TTarget> : CustomInspectorBase<TTarget> where TTarget : StyleSetterBase
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            if (targets.Length <= 1 && target.editInScript != null)
            {
                EditorGUILayout.HelpBox(TryGetText("inspector.style_setter_base.warning").Replace("{name}", target.editInScript.Name), MessageType.Warning);
                DrawLine();
            }

            TargetsSetValue
            (
                x => x.nameSpace,
                x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), x.nameSpace),
                (x, y) => x.nameSpace = y, targets
            );

            TargetsSetValue
            (
                x => x.key,
                x => UsePropertyAndDrawStringArray(serializedObject, "_path", TryGetText("gui.key"), x.key, ThemeLoader.GetStyleKeys(x.nameSpace)),
                (x, y) => x.key = y, targets
            );

            if (targets.Length > 1)
                return;

            ThemeStyle? style = ThemeLoader.GetStyle(target.key, target.nameSpace);
            if (style != null)
            {
                DrawLine();

                RectOffset margin = EditorStyles.helpBox.margin;
                EditorStyles.helpBox.margin = (UnityEngine.RectOffset)new RectOffset(8, 8, 0, 8);

                if (ThemeProjectSettings.themeStyleEditor.DrawGUI(style))
                {
                    string? path = ThemeLoader.GetStylePath(target.key, target.nameSpace);
                    if (path != null)
                        File.WriteAllText(path, JsonManager.ToJson(style));

                    if (target.isActiveAndEnabled)
                        target.Refresh();
                }

                EditorStyles.helpBox.margin = (UnityEngine.RectOffset)margin;
            }
        }
    }
}
