#nullable enable
using RuniEngine.Json;
using RuniEngine.Resource;
using RuniEngine.Resource.Themes;
using RuniEngine.UI.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class ThemeProjectSettings : SettingsProvider
    {
        public static GUIStyle labelStyle
        {
            get
            {
                _labelStyle ??= new GUIStyle(EditorStyles.label)
                {
                    fontSize = 15,
                    hover = new GUIStyleState()
                    {
                        textColor = new Color(0, 0.2352941176f, 0.5333333333f)
                    },
                    active = new GUIStyleState()
                    {
                        textColor = new Color(0, 0.2352941176f * 2, 0.5333333333f * 2)
                    }
                };

                return _labelStyle;
            }
        }

        static GUIStyle? _labelStyle;

        public static IThemeStyleEditor themeStyleEditor { get; set; } = new ThemeStyleEditor();

        public ThemeProjectSettings(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new ThemeProjectSettings("Runi Engine/Theme Setting", SettingsScope.Project);



        string nameSpace = ResourceManager.defaultNameSpace;
        string key = "";
        public override void OnGUI(string searchContext)
        {
            BeginLabelWidth(150);
            DrawGUI(ref nameSpace, ref key);
            EndLabelWidth();
        }

        public static void DrawGUI(ref string nameSpace, ref string key)
        {
            nameSpace = DrawNameSpace(TryGetText("gui.namespace"), nameSpace);
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string nameSpacePath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace);
            string themeFolderPath = Path.Combine(nameSpacePath, ThemeLoader.name);
            
            if (!Directory.Exists(nameSpacePath))
                return;

            string[]? keys = ThemeLoader.GetStyleKeys(nameSpace);
            if (keys == null)
            {
                if (GUILayout.Button(TryGetText("project_setting.theme.styles_folder_create")))
                {
                    Directory.CreateDirectory(themeFolderPath);
                    AssetDatabase.Refresh();
                }

                return;
            }

            {
                GUILayout.BeginHorizontal();

                key = DrawStringArray(TryGetText("gui.key"), key, keys, out int index);

                GUILayout.EndHorizontal();

                if (index < 0)
                {
                    string path = Path.Combine(themeFolderPath, key.Trim());
                    EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(key) || ResourceManager.FileExtensionExists(path, out _, ExtensionFilter.textFileFilter));

                    if (GUILayout.Button(TryGetText("project_setting.theme.styles_file_create")))
                    {
                        File.WriteAllText(path + ".json", "{ }");
                        AssetDatabase.Refresh();
                    }

                    EditorGUI.EndDisabledGroup();
                    return;
                }
            }

            DrawLine();

            {
                ThemeStyle? style = ThemeLoader.GetStyle(key, nameSpace);
                if (style == null)
                    return;

                BeginLabelWidth(EditorGUIUtility.labelWidth * 0.75f);
                RectOffset margin = EditorStyles.helpBox.margin;
                EditorStyles.helpBox.margin = (UnityEngine.RectOffset)new RectOffset(8, 8, 0, 8);

                if (DrawStyleGUI(TryGetText(themeStyleEditor.label), "root", () => themeStyleEditor.DrawGUI(style)))
                {
                    string? path = ThemeLoader.GetStylePath(key, nameSpace);
                    if (path != null)
                        File.WriteAllText(path, JsonManager.ToJson(style));
                }

                EditorStyles.helpBox.margin = (UnityEngine.RectOffset)margin;
                EndLabelWidth();
            }
        }

        static readonly Dictionary<string, bool> styleGUIFold = new Dictionary<string, bool>();
        /// <returns>GUI.changed</returns>
        public static bool DrawStyleGUI(string label, string foldKey, Action drawAction)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (!styleGUIFold.TryGetValue(foldKey, out bool value))
                value = false;

            if (GUILayout.Button(label, labelStyle))
                styleGUIFold[foldKey] = !value;

            bool changed = false;
            if (value)
            {
                EditorGUI.BeginChangeCheck();
                drawAction();
                changed = EditorGUI.EndChangeCheck();
            }

            GUILayout.EndVertical();
            return changed;
        }

        public static void BeginStyleFieldGUI() => GUILayout.BeginVertical(EditorStyles.helpBox);
        public static void EndStyleFieldGUI() => GUILayout.EndVertical();
    }
}
