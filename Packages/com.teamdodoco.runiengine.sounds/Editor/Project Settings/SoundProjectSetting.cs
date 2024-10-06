using RuniEngine.Jsons;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public abstract class SoundProjectSetting : SettingsProvider
    {
        protected SoundProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        public abstract string folderPath { get; }

        public abstract string folderCreateButtonText { get; }
        public abstract string jsonFileCreateButtonText { get; }

        public abstract Type targetDataType { get; }



        [SerializeField] string nameSpace = ResourceManager.defaultNameSpace;
        [SerializeField] bool deleteSafety = true;

        [SerializeField] string selectedKey = "";
        string lastJsonPath = "";

        AudioProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();

        EditorGUISplitView? splitView;

        readonly RuniAdvancedDropdown nameSpaceDropdown = new RuniAdvancedDropdown();
        SoundDataGUI? soundDataGUI;
        readonly List<SoundData> selectedDatas = new List<SoundData>();
        public override void OnGUI(string searchContext)
        {
            soundDataGUI ??= CreateSoundDataGUI();

            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            DeleteSafetyLayout(ref deleteSafety);

            nameSpace = DrawNameSpace(nameSpaceDropdown, TryGetText("gui.namespace"), nameSpace);
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, folderPath);
            if (!Directory.Exists(path))
            {
                if (GUILayout.Button(folderCreateButtonText, GUILayout.ExpandWidth(false)))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }

                EndLabelWidth();
                return;
            }

            string jsonPath = path + ".json";
            if (!File.Exists(jsonPath))
            {
                if (GUILayout.Button(jsonFileCreateButtonText, GUILayout.ExpandWidth(false)))
                {
                    File.WriteAllText(jsonPath, "{}");
                    AssetDatabase.Refresh();
                }

                EndLabelWidth();
                return;
            }

            DrawLine();

            bool isChanged = false;
            IDictionary<string, SoundData>? soundDatas = GetSoundDatas(jsonPath);
            if (soundDatas == null)
            {
                EndLabelWidth();
                return;
            }

            if (treeView == null || lastJsonPath != jsonPath)
            {
                if (treeView != null)
                    treeView.selectionChanged -= TreeViewSelectionChanged;

                treeViewState = new TreeViewState();
                treeView = new AudioProjectSettingTreeView(treeViewState);

                treeView.selectionChanged += TreeViewSelectionChanged;

                if (soundDatas.Count > 0)
                {
                    treeView.keyList = soundDatas.Keys.ToArray();
                    treeView.Reload();
                }

                lastJsonPath = jsonPath;
            }

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
                    bool containsKey = soundDatas.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        soundDatas.Add(selectedKey, CreateEmptySoundData());
                        soundDatas = OrderBy(soundDatas);

                        treeView.keyList = soundDatas.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(!containsKey || (deleteSafety && (soundDatas[selectedKey]?.sounds.Any() ?? false)));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                    {
                        soundDatas.Remove(selectedKey);

                        if (soundDatas.Count > 0)
                        {
                            treeView.keyList = soundDatas.Keys.ToArray();
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

            //오디오 리스트

            DrawLine();

            splitView ??= new EditorGUISplitView(EditorGUISplitView.Direction.Vertical, 0.2f, Repaint);
            splitView.BeginSplitView();

            if (soundDatas.Count > 0)
                treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(splitView.availableRect.height * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            if (treeView != null)
            {
                selectedDatas.Clear();

                for (int i = 0; i < treeView.selectedItems.Length; i++)
                {
                    RuniDictionaryTreeViewItem? item = treeView.selectedItems[i];
                    if (item == null || !soundDatas.ContainsKey(item.key))
                        continue;

                    SoundData? soundData = soundDatas[item.key];
                    if (soundData == null)
                        continue;

                    selectedDatas.Add(soundData);
                }

                //선택된 사운드 데이터와 선택된 트리뷰 아이템 개수가 같지 않을경우, 없는 키가 선택되었다는것을 의미하며 이때는 렌더링되지 않아야합니다
                if (selectedDatas.Count == treeView.selectedItems.Length && selectedDatas.Count > 0)
                {
                    EditorGUILayout.BeginVertical(otherHelpBoxStyle);
                    BeginFieldWidth(10);

                    string key = treeView.selectedItem?.key ?? string.Empty;

                    EditorGUI.BeginChangeCheck();

                    Rect rect = EditorGUILayout.GetControlRect(false, soundDataGUI.GetHeight());
                    soundDataGUI.DrawGUI(rect, nameSpace, key, selectedDatas.ToArray(), out string editedKey);

                    if (EditorGUI.EndChangeCheck())
                        isChanged |= true;

                    EndFieldWidth();
                    EditorGUILayout.EndVertical();

                    if (key != editedKey && !soundDatas.ContainsKey(editedKey))
                    {
                        if (key == selectedKey)
                            selectedKey = editedKey;

                        soundDatas.RenameKey(key, editedKey);
                        soundDatas = OrderBy(soundDatas);

                        treeView.keyList = soundDatas.Keys.ToArray();
                        treeView.Reload();

                        isChanged |= true;
                    }
                }
            }

            splitView.EndSplitView();


            //키 중복 감지 및 리스트를 딕셔너리로 변환
            if (isChanged)
            {
                bool overlap = soundDatas.Count != soundDatas.Distinct(new SoundDataEqualityComparer()).Count();
                if (!overlap)
                    File.WriteAllText(jsonPath, JsonManager.ToJson(soundDatas));
            }

            EndLabelWidth();
        }

        protected abstract SoundDataGUI CreateSoundDataGUI();

        protected abstract IDictionary<string, SoundData>? GetSoundDatas(string path);
        protected abstract SoundData CreateEmptySoundData();

        void TreeViewSelectionChanged(IList<int> selectedIDs)
        {
            if (treeView == null)
                return;

            RuniDictionaryTreeViewItem? item = treeView.FindItem(selectedIDs.Last());
            if (item != null)
                selectedKey = item.key;
        }

        Dictionary<string, T> OrderBy<T>(IDictionary<string, T> list) => list.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        class SoundDataEqualityComparer : IEqualityComparer<KeyValuePair<string, SoundData>>
        {
            public bool Equals(KeyValuePair<string, SoundData> x, KeyValuePair<string, SoundData> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string, SoundData> obj) => obj.Key.GetHashCode();
        }
    }
}
