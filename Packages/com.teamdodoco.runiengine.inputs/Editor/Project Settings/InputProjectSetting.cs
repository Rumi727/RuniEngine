using RuniEngine.Datas;
using RuniEngine.Inputs;
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
        public static SettingsProvider CreateSettingsProvider() => instance ??= new InputProjectSetting("Runi Engine/Input Setting", SettingsScope.Project);



        InputProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        [SerializeField] bool deleteSafety = true;
        ReorderableList? reorderableList;
        InputManager.ProjectData? projectData;
        [SerializeField] string nameSpace = "";
        RuniAdvancedDropdown? nameSpaceDropdown;
        List<List<RuniAdvancedDropdown>> keyCodesDropdownsList = new List<List<RuniAdvancedDropdown>>();
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

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
                inputProjectSetting ??= new StorableClass(typeof(InputManager.ProjectData));
                inputProjectSetting.AutoNameLoad(jsonFolderPath);
            }

            Dictionary<string, List<KeyCode[]>> list = InputManager.ProjectData.controlList;

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
                        keyCodesDropdownsList.Clear();
                    }
                }

                {
                    bool containsKey = list.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        list.Add(selectedKey, new List<KeyCode[]>());
                        OrderBy();

                        treeView.keyList = list.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(!containsKey || (deleteSafety && list[selectedKey].Count > 0));

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

                    DrawGUI(item.key, ref reorderableList, keyCodesDropdownsList, out string editedKey, IsChanged);
                    if (item.key != editedKey)
                    {
                        if (item.key == selectedKey)
                            selectedKey = editedKey;

                        OrderBy();

                        treeView.keyList = list.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        reorderableList = null;
                        keyCodesDropdownsList.Clear();

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
        public static void DrawGUI(string key, ref ReorderableList? reorderableList, List<List<RuniAdvancedDropdown>> keyCodesDropdownsList, out string editedKey, Action? onChangedCallback = null)
        {
            EditorGUILayout.BeginVertical(otherHelpBoxStyle);

            string oldKey = key;

            key = EditorGUILayout.DelayedTextField(TryGetText("gui.key"), key);
            if (InputManager.ProjectData.controlList.ContainsKey(key))
                key = oldKey;

            editedKey = key;

            if (oldKey != key)
                InputManager.ProjectData.controlList.RenameKey(oldKey, key);

            List<KeyCode[]> keyCodesList = InputManager.ProjectData.controlList[key];

            reorderableList ??= new ReorderableList(keyCodesList, typeof(List<KeyCode[]>), true, false, true, true);
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 1;
            reorderableList.onChangedCallback = x => onChangedCallback?.Invoke();
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                List<KeyCode> keyCodes = keyCodesList[index].ToList();
                float x = rect.x;
                float width = rect.width;
                
                rect.width = ((width - 40 - ((keyCodes.Count - 1) * 16)) / (keyCodes.Count)).Clamp(0, 100);
                rect.y += 2;

                bool isChanged = false;
                for (int i = 0; i < keyCodes.Count + 1; i++)
                {
                    if (i < keyCodes.Count)
                    {
                        {
                            while (index >= keyCodesDropdownsList.Count)
                                keyCodesDropdownsList.Add(new List<RuniAdvancedDropdown>());

                            while (i >= keyCodesDropdownsList[index].Count)
                                keyCodesDropdownsList[index].Add(new RuniAdvancedDropdown() { minimumSize = new Vector2(150, 0), maximumSize = new Vector2(0, 350) });
                            
                            KeyCode keyCode = keyCodes[i];
                            keyCodes[i] = keyCodesDropdownsList[index][i].Draw(rect, keyCode);

                            if (keyCode != keyCodes[i])
                                isChanged |= true;
                        }
                        
                        rect.x += rect.width + 2;

                        if (i < keyCodes.Count - 1)
                        {
                            Rect textRect = rect;
                            textRect.y -= 3;

                            GUI.Label(textRect, "+");

                            rect.x += 14;
                        }
                    }
                    else
                    {
                        Rect buttonRect = rect;

                        buttonRect.x = x + width - 36;
                        buttonRect.width = 18;
                        buttonRect.height -= 3;

                        if (GUI.Button(buttonRect, "+"))
                        {
                            keyCodes.Add(KeyCode.None);
                            isChanged |= true;

                            break;
                        }

                        buttonRect.x += 20;

                        EditorGUI.BeginDisabledGroup(keyCodes.Count <= 1);

                        if (GUI.Button(buttonRect, "-"))
                        {
                            keyCodes.RemoveAt(keyCodes.Count - 1);
                            isChanged |= true;

                            break;
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                }

                if (isChanged)
                {
                    keyCodesList[index] = keyCodes.ToArray();
                    onChangedCallback?.Invoke();

                    keyCodesDropdownsList.Clear();
                }
            };
            reorderableList.onAddCallback = x =>
            {
                keyCodesList.Add(new KeyCode[1]);
                x.Select(keyCodesList.Count - 1);
            };

            reorderableList.list = keyCodesList;
            reorderableList.DoLayoutList();

            

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
            keyCodesDropdownsList.Clear();
        }

        void OrderBy() => InputManager.ProjectData.controlList = InputManager.ProjectData.controlList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }
}
