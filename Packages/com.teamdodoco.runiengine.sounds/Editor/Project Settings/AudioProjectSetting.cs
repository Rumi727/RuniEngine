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

#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif

namespace RuniEngine.Editor.ProjectSettings
{
    public class AudioProjectSetting : SettingsProvider
    {
        public AudioProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new AudioProjectSetting("Runi Engine/Resource/Audio Setting", SettingsScope.Project);



        [SerializeField] string nameSpace = ResourceManager.defaultNameSpace;
        [SerializeField] bool deleteSafety = true;
        AudioProjectSettingTreeView? treeView;
        [SerializeField] TreeViewState treeViewState = new TreeViewState();
        EditorGUISplitView? splitView;
        [SerializeField] string selectedKey = "";
        string lastJsonPath = "";
        readonly RuniAdvancedDropdown nameSpaceDropdown = new RuniAdvancedDropdown();
        public override void OnGUI(string searchContext)
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            DeleteSafetyLayout(ref deleteSafety);

            nameSpace = DrawNameSpace(nameSpaceDropdown, TryGetText("gui.namespace"), nameSpace);
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string nameSpace2 = nameSpace;
            bool deleteSafety2 = deleteSafety;

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, AudioLoader.name);
            if (!Directory.Exists(path))
            {
                if (GUILayout.Button(TryGetText("project_setting.audio.audios_folder_create"), GUILayout.ExpandWidth(false)))
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
                if (GUILayout.Button(TryGetText("project_setting.audio.audios_file_create"), GUILayout.ExpandWidth(false)))
                {
                    File.WriteAllText(jsonPath, "{}");
                    AssetDatabase.Refresh();
                }

                EndLabelWidth();
                return;
            }

            DrawLine();

            bool isChanged = false;
            Dictionary<string, AudioData?>? audioDatas = JsonManager.JsonRead<Dictionary<string, AudioData?>>(jsonPath);
            if (audioDatas == null)
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

                if (audioDatas.Count > 0)
                {
                    treeView.keyList = audioDatas.Keys.ToArray();
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
                    bool containsKey = audioDatas.ContainsKey(selectedKey);
                    EditorGUI.BeginDisabledGroup(containsKey);

                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        audioDatas.Add(selectedKey, new AudioData("", false));
                        audioDatas = OrderBy(audioDatas);

                        treeView.keyList = audioDatas.Keys.ToArray();

                        treeView.Reload();
                        treeView.SetSelection(selectedKey);

                        isChanged |= true;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(!containsKey || (deleteSafety && (audioDatas[selectedKey]?.audios.Any() ?? false)));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                    {
                        audioDatas.Remove(selectedKey);

                        if (audioDatas.Count > 0)
                        {
                            treeView.keyList = audioDatas.Keys.ToArray();
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

            if (audioDatas.Count > 0)
                treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(splitView.availableRect.height * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            if (treeView != null)
            {
                for (int i = 0; i < treeView.selectedItems.Length; i++)
                {
                    RuniDictionaryTreeViewItem? item = treeView.selectedItems[i];
                    if (item == null || !audioDatas.ContainsKey(item.key))
                        continue;

                    AudioData? audioData = audioDatas[item.key];
                    if (audioData == null)
                        continue;

                    EditorGUILayout.BeginVertical(otherHelpBoxStyle);
                    BeginFieldWidth(10);

                    audioDatas[item.key] = DrawGUI(nameSpace, item.key, audioData, deleteSafety, out bool isChanged2, out string editedKey);

                    EndFieldWidth();
                    EditorGUILayout.EndVertical();

                    if (item.key != editedKey && !audioDatas.ContainsKey(editedKey))
                    {
                        if (item.key == selectedKey)
                            selectedKey = editedKey;

                        audioDatas.RenameKey(item.key, editedKey);
                        audioDatas = OrderBy(audioDatas);

                        treeView.keyList = audioDatas.Keys.ToArray();
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
                bool overlap = audioDatas.Count != audioDatas.Distinct(new AudioDataEqualityComparer()).Count();
                if (!overlap)
                    File.WriteAllText(jsonPath, JsonManager.ToJson(audioDatas.ToDictionary(x => x.Key ?? "", x => x.Value)));
            }

            EndLabelWidth();
        }

        public static AudioData DrawGUI(string nameSpace, string key, AudioData audioData, bool deleteSafety, out bool isChanged, out string editedKey)
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

                subtitle = EditorGUILayout.TextField(TryGetText("gui.subtitle"), audioData.subtitle);
                isBGM = EditorGUILayout.Toggle("is BGM", audioData.isBGM);

                tempIsChanged = tempIsChanged || EditorGUI.EndChangeCheck();
                EndLabelWidth();
            }

            List<AudioMetaData> metaDatas = audioData.audios.ToList();

            //오디오 메타데이터 리스트
            DrawRawList(metaDatas, "", y =>
            {
                AudioMetaData metaData = (AudioMetaData)y;
                string audioPath = metaData.path;
                double pitch = metaData.pitch;
                double tempo = metaData.tempo;
                int loopStartIndex = metaData.loopStartIndex;
                int loopOffsetIndex = metaData.loopOffsetIndex;
#if ENABLE_RUNI_ENGINE_RHYTHMS
                int rhythmOffsetIndex = metaData.rhythmOffsetIndex;
                BeatBPMPairList bpms = new BeatBPMPairList(metaData.bpms.defaultValue, metaData.bpms);
#endif

                string realAudioPath = "";

                {
                    EditorGUILayout.BeginHorizontal();
                    string label = TryGetText("gui.path");

                    BeginLabelWidth(label);
                    EditorGUI.BeginChangeCheck();

                    audioPath = EditorGUILayout.TextField(label, audioPath);

                    tempIsChanged |= EditorGUI.EndChangeCheck();
                    EndLabelWidth();

                    //GUI
                    {
                        string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, AudioLoader.name).Replace("\\", "/");
                        string assetAllPathAndName = Path.Combine(assetAllPath, audioPath).Replace("\\", "/");

                        string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace, AudioLoader.name).Replace("\\", "/");
                        string assetPathAndName = Path.Combine(assetPath, audioPath).Replace("\\", "/");

                        ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ExtensionFilter.musicFileFilter);
                        realAudioPath = outPath;

                        EditorGUI.BeginChangeCheck();

                        DefaultAsset audioClip;
                        if (audioPath != "")
                        {
                            audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false, GUILayout.Width(100));
                        }
                        else
                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false, GUILayout.Width(100));

                        tempIsChanged = tempIsChanged || EditorGUI.EndChangeCheck();

                        string changedAssetPathAneName = AssetDatabase.GetAssetPath(audioClip).Replace(assetPath + "/", "");
                        for (int k = 0; k < ExtensionFilter.musicFileFilter.extensions.Length; k++)
                        {
                            if (Path.GetExtension(changedAssetPathAneName) == ExtensionFilter.musicFileFilter.extensions[k])
                            {
                                audioPath = PathUtility.GetPathWithExtension(changedAssetPathAneName).Replace("\\", "/");
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

                    Space(5);

                    {
                        string label = TryGetText("gui.tempo");
                        BeginLabelWidth(label);

                        tempo = EditorGUILayout.DoubleField(label, tempo);
                        EndLabelWidth();
                    }

                    Space(5);

                    //타입
                    {
                        string jsonPath = realAudioPath + ".json";
                        
                        AudioFileMetaData? fileMetaData = JsonManager.JsonRead<AudioFileMetaData>(jsonPath);
                        RawAudioLoadType loadType = RawAudioLoadType.instant;
                        if (fileMetaData != null)
                            loadType = fileMetaData.Value.loadType;

                        EditorGUI.BeginChangeCheck();

                        {
                            string label = TryGetText("gui.load");
                            BeginLabelWidth(label);

                            loadType = (RawAudioLoadType)EditorGUILayout.EnumPopup(label, loadType, GUILayout.Width(140));
                            EndLabelWidth();
                        }

                        if (EditorGUI.EndChangeCheck() && File.Exists(realAudioPath))
                        {
                            if (loadType == RawAudioLoadType.instant)
                            {
                                if (File.Exists(jsonPath))
                                {
                                    File.Delete(jsonPath);
                                    File.Delete(jsonPath + ".meta");

                                    AssetDatabase.Refresh();
                                }
                            }
                            else
                            {
                                File.WriteAllText(jsonPath, JsonManager.ToJson(new AudioFileMetaData(loadType)));
                                AssetDatabase.Refresh();
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    {
                        string label = TryGetText("project_setting.audio.loop_start_index");
                        BeginLabelWidth(label);

                        loopStartIndex = EditorGUILayout.IntField(label, loopStartIndex).Clamp(0);
                        EndLabelWidth();
                    }

                    Space(5);

                    {
                        string label = TryGetText("project_setting.audio.loop_offset_index");
                        BeginLabelWidth(label);

                        loopOffsetIndex = EditorGUILayout.IntField(label, loopOffsetIndex).Clamp(0);
                        EndLabelWidth();
                    }

                    EditorGUILayout.EndHorizontal();
                    tempIsChanged |= EditorGUI.EndChangeCheck();
                }

#if ENABLE_RUNI_ENGINE_RHYTHMS
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    {
                        string label = TryGetText("project_setting.audio.rhythm_offset_index");
                        BeginLabelWidth(label);

                        rhythmOffsetIndex = EditorGUILayout.IntField(label, rhythmOffsetIndex);
                        EndLabelWidth();
                    }

                    tempIsChanged |= EditorGUI.EndChangeCheck();
                    EditorGUILayout.EndHorizontal();

                    DrawRawList(bpms, "", x =>
                    {
                        EditorGUI.BeginChangeCheck();

                        var pair = (BeatValuePair<BPM>)x;
                        double beat = EditorGUILayout.DelayedDoubleField(TryGetText("gui.beat"), pair.beat);
                        double value = EditorGUILayout.DoubleField(TryGetText("gui.value"), pair.value.bpm);
                        double timeSignatures = EditorGUILayout.DoubleField(TryGetText("gui.time_signatures"), pair.value.timeSignatures);

                        tempIsChanged |= EditorGUI.EndChangeCheck();
                        
                        return new BeatValuePair<BPM>(beat, new(value, timeSignatures));
                    }, i => true, i =>
                    {
                        if (bpms.Count > 0)
                            bpms.Insert(i, new BeatValuePair<BPM>(bpms[i.Clamp(0, bpms.Count - 1)].beat, new(60)));
                        else    
                            bpms.Insert(i, new BeatValuePair<BPM>(0, new(60)));
                    }, out bool isListChanged);

                    tempIsChanged |= isListChanged;
                }
#endif

#if ENABLE_RUNI_ENGINE_RHYTHMS
                return new AudioMetaData(audioPath, pitch, tempo, loopStartIndex, loopOffsetIndex, bpms, rhythmOffsetIndex, null);
#else
                return new AudioMetaData(audioPath, pitch, tempo, loopStartIndex, loopOffsetIndex, null);
#endif
            }, i => string.IsNullOrEmpty(metaDatas[i].path), i => metaDatas.Insert(i,
#if ENABLE_RUNI_ENGINE_RHYTHMS
            new AudioMetaData("", 1, 1, 0, 0, null, 0, null)
#else
            new AudioMetaData("", 1, 1, 0, 0, null)
#endif
            ), out bool isListChanged, deleteSafety);

            isChanged = tempIsChanged || isListChanged;
            return new AudioData(subtitle, isBGM, metaDatas.ToArray());
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

        class AudioDataEqualityComparer : IEqualityComparer<KeyValuePair<string, AudioData?>>
        {
            public bool Equals(KeyValuePair<string, AudioData?> x, KeyValuePair<string, AudioData?> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string, AudioData?> obj) => obj.Key.GetHashCode();
        }
    }
}
