#nullable enable
using RuniEngine.Datas;
using RuniEngine.Inputs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class InputProjectSetting : SettingsProvider
    {
        public InputProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new InputProjectSetting("Runi Engine/Input Setting", SettingsScope.Project);



        public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                inputProjectSetting ??= new StorableClass(typeof(InputManager.ProjectData));
                inputProjectSetting.AutoNameLoad(Kernel.projectSettingPath);
            }
            
            InputManager.ProjectData.controlList ??= new Dictionary<string, KeyCode[]>();
            if (InputManager.ProjectData.controlList.Count > 0)
                treeView?.Reload();
        }

        InputProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        [SerializeField] bool deleteSafety = true;
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            Dictionary<string, KeyCode[]> list = InputManager.ProjectData.controlList;

            if (treeView == null)
            {
                treeView = new InputProjectSettingTreeView(treeViewState);
                treeView.selectionChanged += TreeViewSelectionChanged;

                if (list.Count > 0)
                    treeView.Reload();
            }

            DeleteSafety(ref deleteSafety);

            bool isChanged = false;

            {
                EditorGUILayout.BeginHorizontal();

                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.label);
                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.textField);

                {
                    EditorGUI.BeginChangeCheck();

                    selectedKey = EditorGUILayout.TextField(TryGetText("gui.key"), selectedKey, GUILayout.Height(GetButtonYSize()));

                    if (EditorGUI.EndChangeCheck() && treeView.ContainsKey(selectedKey))
                        treeView.SetSelection(selectedKey);
                }

                {
                    bool containsKey = list.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        list.Add(selectedKey, new KeyCode[0]);
                        OrderBy();

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

                    if (list.ContainsKey(item.key))
                    {
                        DrawGUI(item.key, deleteSafety, out bool isListChanged, out string editedKey);
                        if (item.key != editedKey)
                        {
                            if (item.key == selectedKey)
                                selectedKey = editedKey;

                            OrderBy();
                            treeView.Reload();

                            isChanged |= true;
                        }

                        isChanged |= isListChanged;
                    }
                }
            }

            splitView.EndSplitView();

            if (isChanged && !Kernel.isPlaying && inputProjectSetting != null)
                inputProjectSetting.AutoNameSave(Kernel.projectSettingPath);

            EndLabelWidth();
        }

        public static StorableClass? inputProjectSetting = null;
        public static void DrawGUI(string key, bool deleteSafety, out bool isListChanged, out string editedKey)
        {
            EditorGUILayout.BeginVertical(otherHelpBox);

            string oldKey = key;

            key = EditorGUILayout.DelayedTextField(TryGetText("gui.key"), key);
            if (InputManager.ProjectData.controlList.ContainsKey(key))
                key = oldKey;

            editedKey = key;

            if (oldKey != key)
                InputManager.ProjectData.controlList.RenameKey(oldKey, key);

            List<KeyCode> keyCodes = InputManager.ProjectData.controlList[key].ToList();
            DrawRawList
            (
                keyCodes,
                "",
                x => EditorGUILayout.EnumPopup(TryGetText("gui.key"), (KeyCode)x),
                x => keyCodes[x] == KeyCode.None,
                x => keyCodes.Insert(x, KeyCode.None),
                out isListChanged,
                deleteSafety
            );

            InputManager.ProjectData.controlList[key] = keyCodes.ToArray();
            EditorGUILayout.EndVertical();
        }

        void TreeViewSelectionChanged(IList<int> selectedIDs)
        {
            if (treeView == null)
                return;

            RuniDictionaryTreeViewItem? item = treeView.FindItem(selectedIDs.Last());
            if (item != null)
                selectedKey = item.key;
        }

        void OrderBy() => InputManager.ProjectData.controlList = InputManager.ProjectData.controlList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }
}
