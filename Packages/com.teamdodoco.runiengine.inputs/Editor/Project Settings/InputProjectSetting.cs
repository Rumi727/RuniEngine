#nullable enable
using RuniEngine.Datas;
using RuniEngine.Inputs;
using RuniEngine.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    [Serializable]
    public class InputProjectSetting : SettingsProvider
    {
        public InputProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new InputProjectSetting("Runi Engine/Namespace/Input Setting", SettingsScope.Project);



        InputProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        [SerializeField] bool deleteSafety = true;
        ReorderableList? reorderableList;
        InputManager.ProjectData? projectData;
        [SerializeField] string nameSpace = "";
        RuniAdvancedDropdown? nameSpaceDropdown;
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            {
                nameSpace = DrawStringArray(ref nameSpaceDropdown, TryGetText("gui.namespace"), nameSpace, SettingManager.GetNameSpaces());
                if (string.IsNullOrEmpty(nameSpace))
                    return;
            }

            string jsonFolderPath = Path.Combine(Kernel.projectSettingPath, nameSpace);
            if (!Directory.Exists(jsonFolderPath))
            {
                if (GUILayout.Button(TryGetText("gui.folder_create")))
                    Directory.CreateDirectory(jsonFolderPath);

                return;
            }

            DrawLine();

            if (!Kernel.isPlaying)
            {
                inputProjectSetting ??= new StorableClass(new InputManager.ProjectData());
                inputProjectSetting.AutoNameLoad(jsonFolderPath);

                projectData = (InputManager.ProjectData)(inputProjectSetting.instance ?? new InputManager.ProjectData());
            }
            else
                projectData = SettingManager.GetProjectSetting<InputManager.ProjectData>(nameSpace) ?? new InputManager.ProjectData();

            Dictionary<string, KeyCode[]> list = projectData.controlList;

            if (treeView == null)
            {
                treeView = new InputProjectSettingTreeView(treeViewState);
                treeView.selectionChanged += TreeViewSelectionChanged;

                if (list.Count > 0)
                {
                    treeView.keyList = list.Keys.ToArray();
                    treeView.Reload();
                }
            }

            DeleteSafetyLayout(ref deleteSafety);

            bool isChanged = false;

            {
                EditorGUILayout.BeginHorizontal();

                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.label);
                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.textField);

                {
                    EditorGUI.BeginChangeCheck();

                    selectedKey = EditorGUILayout.TextField(TryGetText("gui.key"), selectedKey, GUILayout.Height(GetButtonYSize()));

                    if (EditorGUI.EndChangeCheck() && treeView.ContainsKey(selectedKey))
                    {
                        treeView.SetSelection(selectedKey);
                        reorderableList = null;
                    }
                }

                {
                    bool containsKey = list.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        list.Add(selectedKey, new KeyCode[0]);
                        OrderBy();

                        treeView.keyList = list.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(!containsKey || (deleteSafety && list[selectedKey].Length > 0));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                    {
                        list.Remove(selectedKey);

                        if (list.Count > 0)
                        {
                            treeView.keyList = list.Keys.ToArray();
                            treeView.Reload();
                            
                            TreeViewSelectionChanged(treeView.GetSelection());
                        }

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                EndAlignment(EditorStyles.textField);
                EndAlignment(EditorStyles.label);

                EditorGUILayout.EndHorizontal();
            }

            DrawLine();

            splitView ??= new EditorGUISplitView(EditorGUISplitView.Direction.Vertical, 0.2f, Repaint);
            splitView.BeginSplitView();

            if (list.Count > 0)
                treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(splitView.availableRect.height * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            if (treeView != null)
            {
                for (int i = 0; i < treeView.selectedItems.Length; i++)
                {
                    RuniDictionaryTreeViewItem? item = treeView.selectedItems[i];
                    if (item == null)
                        continue;

                    if (!list.ContainsKey(item.key))
                        continue;

                    DrawGUI(projectData, item.key, ref reorderableList, out string editedKey, IsChanged);
                    if (item.key != editedKey)
                    {
                        if (item.key == selectedKey)
                            selectedKey = editedKey;

                        OrderBy();

                        treeView.keyList = list.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        reorderableList = null;
                        isChanged |= true;
                    }
                }
            }

            splitView.EndSplitView();

            if (isChanged)
                IsChanged();

            void IsChanged()
            {
                if (!Kernel.isPlaying && inputProjectSetting != null)
                    inputProjectSetting.AutoNameSave(jsonFolderPath);
            }

            EndLabelWidth();
        }

        public static StorableClass? inputProjectSetting = null;
        public static void DrawGUI(InputManager.ProjectData projectData, string key, ref ReorderableList? reorderableList, out string editedKey, Action? onChangedCallback = null)
        {
            EditorGUILayout.BeginVertical(otherHelpBoxStyle);

            string oldKey = key;

            key = EditorGUILayout.DelayedTextField(TryGetText("gui.key"), key);
            if (projectData.controlList.ContainsKey(key))
                key = oldKey;

            editedKey = key;

            if (oldKey != key)
                projectData.controlList.RenameKey(oldKey, key);

            List<KeyCode> keyCodes = projectData.controlList[key].ToList();
            reorderableList ??= new ReorderableList(keyCodes, typeof(List<KeyCode>), true, false, true, true);
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
            reorderableList.onChangedCallback = x =>
            {
                projectData.controlList[key] = keyCodes.ToArray();
                onChangedCallback?.Invoke();
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 1;

                EditorGUI.BeginChangeCheck();
                keyCodes[index] = (KeyCode)EditorGUI.EnumPopup(rect, keyCodes[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    projectData.controlList[key] = keyCodes.ToArray();
                    onChangedCallback?.Invoke();
                }
            };

            reorderableList.list = keyCodes;
            reorderableList.DoLayoutList();

            projectData.controlList[key] = keyCodes.ToArray();

            EditorGUILayout.EndVertical();
        }

        void TreeViewSelectionChanged(IList<int> selectedIDs)
        {
            if (treeView == null)
                return;

            RuniDictionaryTreeViewItem? item = treeView.FindItem(selectedIDs.Last());
            if (item != null)
                selectedKey = item.key;

            reorderableList = null;
        }

        void OrderBy()
        {
            if (projectData != null)
                projectData.controlList = projectData.controlList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
