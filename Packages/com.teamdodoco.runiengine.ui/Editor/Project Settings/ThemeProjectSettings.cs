#nullable enable
using RuniEngine.Jsons;
using RuniEngine.Resource;
using RuniEngine.Resource.Themes;
using RuniEngine.UI.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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

        public override void OnActivate(string searchContext, VisualElement rootElement) => advancedDropdowns.Clear();

        string nameSpace = ResourceManager.defaultNameSpace;
        string key = "";
        static int advancedDropdownsIndex = 0;
        static readonly List<RuniAdvancedDropdown> advancedDropdowns = new List<RuniAdvancedDropdown>();
        public override void OnGUI(string searchContext)
        {
            BeginLabelWidth(150);
            DrawGUI(ref nameSpace, ref key);
            EndLabelWidth();
        }

        public static void DrawGUI(ref string nameSpace, ref string key)
        {
            advancedDropdownsIndex = 0;
            for (int i = 0; i < advancedDropdowns.Count; i++)
                advancedDropdowns[i].Clear();

            nameSpace = DrawNameSpace(GetAdvancedDropdown(), TryGetText("gui.namespace"), nameSpace);
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string nameSpacePath = Path.Combine(ResourceManager.rootName, nameSpace);
            string themeFolderPath = Path.Combine(nameSpacePath, ThemeLoader.name);

            if (!StreamingIOHandler.instance.DirectoryExists(nameSpacePath))
                return;

            string[]? keys = ThemeLoader.GetStyleKeys(StreamingIOHandler.instance, nameSpace);
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

                key = DrawStringArray(GetAdvancedDropdown(), TryGetText("gui.key"), key, keys, false, out int index);

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
                ThemeStyle? style = ThemeLoader.GetStyle(StreamingIOHandler.instance, key, nameSpace);
                if (style == null)
                    return;

                BeginLabelWidth(EditorGUIUtility.labelWidth * 0.75f);
                EditorGUI.BeginChangeCheck();

                themeStyleEditor.DrawGUI(style);

                if (EditorGUI.EndChangeCheck())
                {
                    string? path = ThemeLoader.GetStylePath(StreamingIOHandler.instance, key, nameSpace);
                    if (path != null)
                        File.WriteAllText(path, JsonManager.ToJson(style));
                }

                EndLabelWidth();
            }
        }

        public static RuniAdvancedDropdown GetAdvancedDropdown()
        {
            RuniAdvancedDropdown result;
            if (advancedDropdownsIndex >= advancedDropdowns.Count)
            {
                result = new RuniAdvancedDropdown();
                advancedDropdowns.Add(result);
            }
            else
                result = advancedDropdowns[advancedDropdownsIndex];

            advancedDropdownsIndex++;
            return result;
        }

        static readonly Dictionary<string, bool> styleGUIFold = new Dictionary<string, bool>();
        /// <returns>GUI.changed</returns>
        public static bool DrawStyleGUI(string label, string foldKey, Action drawAction)
        {
            GUILayout.BeginVertical(otherHelpBoxStyle);

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

        public static void BeginStyleFieldGUI() => GUILayout.BeginVertical(otherHelpBoxStyle);
        public static void EndStyleFieldGUI() => GUILayout.EndVertical();
    }
}
