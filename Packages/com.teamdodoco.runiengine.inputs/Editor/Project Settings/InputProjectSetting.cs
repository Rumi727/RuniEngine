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
                inputProjectSetting.AutoNameLoad(Kernel.projectDataPath);
            }
            
            InputManager.ProjectData.controlList ??= new Dictionary<string, KeyCode[]>();
            if (InputManager.ProjectData.controlList.Count > 0)
                treeView?.Reload();
        }

        InputProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        [SerializeField] EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        public override void OnGUI(string searchContext)
        {
            Dictionary<string, KeyCode[]> list = InputManager.ProjectData.controlList;

            if (treeView == null)
            {
                treeView = new InputProjectSettingTreeView(treeViewState);
                treeView.selectionChanged += TreeViewSelectionChanged;

                if (list.Count > 0)
                    treeView.Reload();
            }

            bool isChanged = false;

            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.label);
                BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.textField);

                selectedKey = EditorGUILayout.TextField(TryGetText("gui.key"), selectedKey, GUILayout.Height(GetButtonYSize()));

                if (EditorGUI.EndChangeCheck() && treeView.itemIDs.ContainsKey(selectedKey))
                    treeView.SetSelection(treeView.itemIDs[selectedKey]);

                string addLabel = TryGetText("gui.add");
                string removeLabel = TryGetText("gui.remove");

                {
                    bool containsKey = list.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(addLabel, GUILayout.ExpandWidth(false)))
                    {
                        list.Add(selectedKey, new KeyCode[0]);
                        OrderBy();

                        treeView.Reload();
                        treeView.SetSelection(treeView.itemIDs[selectedKey]);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(!containsKey);

                    if (GUILayout.Button(removeLabel, GUILayout.ExpandWidth(false)))
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
                    InputProjectSettingTreeViewItem? item = treeView.selectedItems[i];
                    if (item == null)
                        continue;

                    if (list.ContainsKey(item.key))
                    {
                        DrawGUI(item.key, out bool isListChanged, out string editedKey);
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
                inputProjectSetting.AutoNameSave(Kernel.projectDataPath);
        }

        public static StorableClass? inputProjectSetting = null;
        public static void DrawGUI(string key, out bool isListChanged, out string editedKey)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            string oldKey = key;

            key = EditorGUILayout.DelayedTextField(TryGetText("gui.key"), key);
            if (InputManager.ProjectData.controlList.ContainsKey(key))
                key = oldKey;

            editedKey = key;

            if (oldKey != key)
                InputManager.ProjectData.controlList.RenameKey(oldKey, key);

            List<KeyCode> keyCodes = InputManager.ProjectData.controlList[key].ToList();
            DrawRawList(keyCodes, "", x => EditorGUILayout.EnumPopup(TryGetText("gui.key"), (KeyCode)x), x => keyCodes[x] == KeyCode.None, x => keyCodes.Insert(x, KeyCode.None), out isListChanged);

            InputManager.ProjectData.controlList[key] = keyCodes.ToArray();
            EditorGUILayout.EndVertical();
        }

        void TreeViewSelectionChanged(IList<int> selectedIDs)
        {
            if (treeView == null)
                return;

            InputProjectSettingTreeViewItem? item = treeView.FindItem(selectedIDs.Last());
            if (item != null)
                selectedKey = item.key;
        }

        void OrderBy() => InputManager.ProjectData.controlList = InputManager.ProjectData.controlList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }
}
