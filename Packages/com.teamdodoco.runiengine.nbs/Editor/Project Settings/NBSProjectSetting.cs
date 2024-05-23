#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System.IO;
using RuniEngine.Jsons;
using UnityEditor.IMGUI.Controls;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class NBSProjectSetting : SettingsProvider
    {
        public NBSProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new NBSProjectSetting("Runi Engine/NBS Setting", SettingsScope.Project);



        [SerializeField] string nameSpace = ResourceManager.defaultNameSpace;
        [SerializeField] bool deleteSafety = true;
        NBSProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        string lastJsonPath = "";
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            DeleteSafety(ref deleteSafety);

            nameSpace = DrawNameSpace(TryGetText("gui.namespace"), nameSpace);
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string nameSpace2 = nameSpace;
            bool deleteSafety2 = deleteSafety;

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, NBSLoader.name);
            if (!Directory.Exists(path))
            {
                if (GUILayout.Button(TryGetText("project_setting.nbs.nbses_folder_create"), GUILayout.ExpandWidth(false)))
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
                if (GUILayout.Button(TryGetText("project_setting.nbs.nbses_file_create"), GUILayout.ExpandWidth(false)))
                {
                    File.WriteAllText(jsonPath, "{}");
                    AssetDatabase.Refresh();
                }

                EndLabelWidth();
                return;
            }

            DrawLine();

            bool isChanged = false;
            Dictionary<string, NBSData?>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData?>>(jsonPath);
            if (nbsDatas == null)
            {
                EndLabelWidth();
                return;
            }

            if (treeView == null || lastJsonPath != jsonPath)
            {
                if (treeView != null)
                    treeView.selectionChanged -= TreeViewSelectionChanged;

                treeViewState = new TreeViewState();
                treeView = new NBSProjectSettingTreeView(treeViewState);

                treeView.selectionChanged += TreeViewSelectionChanged;

                if (nbsDatas.Count > 0)
                {
                    treeView.keyList = nbsDatas.Keys.ToArray();
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
                    bool containsKey = nbsDatas.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        nbsDatas.Add(selectedKey, new NBSData("", false));
                        nbsDatas = OrderBy(nbsDatas);

                        treeView.keyList = nbsDatas.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(!containsKey || (deleteSafety && (nbsDatas[selectedKey]?.nbses.Any() ?? false)));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                    {
                        nbsDatas.Remove(selectedKey);

                        if (nbsDatas.Count > 0)
                        {
                            treeView.keyList = nbsDatas.Keys.ToArray();
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

            if (nbsDatas.Count > 0)
                treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(splitView.availableRect.height * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            if (treeView != null)
            {
                for (int i = 0; i < treeView.selectedItems.Length; i++)
                {
                    RuniDictionaryTreeViewItem? item = treeView.selectedItems[i];
                    if (item == null || !nbsDatas.ContainsKey(item.key))
                        continue;

                    NBSData? nbsData = nbsDatas[item.key];
                    if (nbsData == null)
                        continue;

                    EditorGUILayout.BeginVertical(otherHelpBox);
                    BeginFieldWidth(10);

                    nbsDatas[item.key] = DrawGUI(nameSpace, item.key, nbsData, deleteSafety, out bool isChanged2, out string editedKey);

                    EndFieldWidth();
                    EditorGUILayout.EndVertical();

                    if (item.key != editedKey && !nbsDatas.ContainsKey(editedKey))
                    {
                        if (item.key == selectedKey)
                            selectedKey = editedKey;

                        nbsDatas.RenameKey(item.key, editedKey);
                        nbsDatas = OrderBy(nbsDatas);

                        treeView.keyList = nbsDatas.Keys.ToArray();
                        treeView.Reload();

                        isChanged |= true;
                    }

                    isChanged |= isChanged2;
                }
            }

            splitView.EndSplitView();


            //키 중복 감지 및 리스트를 딕셔너리로 변환
            if (isChanged)
            {
                bool overlap = nbsDatas.Count != nbsDatas.Distinct(new NBSDataEqualityComparer()).Count();
                if (!overlap)
                    File.WriteAllText(jsonPath, JsonManager.ToJson(nbsDatas.ToDictionary(x => x.Key ?? "", x => x.Value)));
            }

            EndLabelWidth();
        }

        public static NBSData DrawGUI(string nameSpace, string key, NBSData nbsData, bool deleteSafety, out bool isChanged, out string editedKey)
        {
            bool tempIsChanged = false;
            EditorGUI.BeginChangeCheck();

            string subtitle;
            bool isBGM;

            {
                BeginLabelWidth(50);
                EditorGUI.BeginChangeCheck();

                key = EditorGUILayout.DelayedTextField(TryGetText("gui.key"), key);
                editedKey = key;

                subtitle = EditorGUILayout.TextField(TryGetText("gui.subtitle"), nbsData.subtitle);
                isBGM = EditorGUILayout.Toggle("is BGM", nbsData.isBGM);

                tempIsChanged = tempIsChanged || EditorGUI.EndChangeCheck();
                EndLabelWidth();
            }

            List<NBSMetaData> metaDatas = nbsData.nbses.ToList();

            //오디오 메타데이터 리스트
            DrawRawList(metaDatas, "", y =>
            {
                NBSMetaData metaData = (NBSMetaData)y;
                string nbsPath = metaData.path;
                double pitch = metaData.pitch;
                double tempo = metaData.tempo;

                {
                    EditorGUILayout.BeginHorizontal();
                    string label = TryGetText("gui.path");

                    BeginLabelWidth(label);
                    EditorGUI.BeginChangeCheck();

                    nbsPath = EditorGUILayout.TextField(label, nbsPath);

                    tempIsChanged |= EditorGUI.EndChangeCheck();
                    EndLabelWidth();

                    //GUI
                    {
                        string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, NBSLoader.name).Replace("\\", "/");
                        string assetAllPathAndName = Path.Combine(assetAllPath, nbsPath).Replace("\\", "/");

                        string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace, NBSLoader.name).Replace("\\", "/");
                        string assetPathAndName = Path.Combine(assetPath, nbsPath).Replace("\\", "/");

                        ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ExtensionFilter.nbsFileFilter);

                        EditorGUI.BeginChangeCheck();

                        DefaultAsset nbsClip;
                        if (nbsPath != "")
                        {
                            nbsClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                            nbsClip = (DefaultAsset)EditorGUILayout.ObjectField(nbsClip, typeof(DefaultAsset), false, GUILayout.Width(100));
                        }
                        else
                            nbsClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false, GUILayout.Width(100));

                        tempIsChanged = tempIsChanged || EditorGUI.EndChangeCheck();

                        string changedAssetPathAneName = AssetDatabase.GetAssetPath(nbsClip).Replace(assetPath + "/", "");
                        for (int k = 0; k < ExtensionFilter.nbsFileFilter.extensions.Length; k++)
                        {
                            if (Path.GetExtension(changedAssetPathAneName) == ExtensionFilter.nbsFileFilter.extensions[k])
                            {
                                nbsPath = PathUtility.GetPathWithExtension(changedAssetPathAneName).Replace("\\", "/");
                                continue;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    {
                        string label = TryGetText("gui.pitch");
                        BeginLabelWidth(label);

                        pitch = EditorGUILayout.DoubleField(label, pitch);
                        EndLabelWidth();
                    }

                    {
                        string label = TryGetText("gui.tempo");
                        BeginLabelWidth(label);

                        tempo = EditorGUILayout.DoubleField(label, tempo);
                        EndLabelWidth();
                    }

                    EditorGUILayout.EndHorizontal();
                    tempIsChanged |= EditorGUI.EndChangeCheck();
                }

                return new NBSMetaData(nbsPath, pitch, tempo, null);
            }, i => string.IsNullOrEmpty(metaDatas[i].path), i => metaDatas.Insert(i, new NBSMetaData("", 1, 1, null)), out bool isListChanged, deleteSafety);

            isChanged = tempIsChanged || isListChanged;
            return new NBSData(subtitle, isBGM, metaDatas.ToArray());
        }

        void TreeViewSelectionChanged(IList<int> selectedIDs)
        {
            if (treeView == null)
                return;

            RuniDictionaryTreeViewItem? item = treeView.FindItem(selectedIDs.Last());
            if (item != null)
                selectedKey = item.key;
        }

        Dictionary<string, T?> OrderBy<T>(Dictionary<string, T?> list) => list.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        class NBSDataEqualityComparer : IEqualityComparer<KeyValuePair<string, NBSData?>>
        {
            public bool Equals(KeyValuePair<string, NBSData?> x, KeyValuePair<string, NBSData?> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string, NBSData?> obj) => obj.Key.GetHashCode();
        }
    }
}
