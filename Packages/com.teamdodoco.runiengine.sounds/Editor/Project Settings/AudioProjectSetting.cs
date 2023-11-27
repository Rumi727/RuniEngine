#nullable enable
/*
 * 이 스크립트는 SC KRM에서 따왔으며, 완벽한 리스트 대체제가 생긴다면 코드를 완전히 갈아엎어야합니다
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System.IO;
using RuniEngine.Json;

namespace RuniEngine.Editor.ProjectSetting
{
    public class AudioProjectSetting : SettingsProvider
    {
        public AudioProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new AudioProjectSetting("Runi Engine/Audio Setting", SettingsScope.Project);



        string nameSpace = "";
        bool deleteSafety = true;
        int displayRestrictionsIndex = 0;
        public override void OnGUI(string searchContext) => DrawGUI(ref nameSpace, ref deleteSafety, ref displayRestrictionsIndex);

        public static void DrawGUI(ref string nameSpace, ref bool deleteSafety, ref int displayRestrictionsIndex)
        {
            EditorTool.BeginFieldWidth(10);
            EditorTool.DeleteSafety(ref deleteSafety);

            nameSpace = EditorTool.DrawNameSpace(EditorTool.TryGetText("gui.namespace"), nameSpace);
            if (string.IsNullOrEmpty(nameSpace))
                return;

            string nameSpace2 = nameSpace;
            bool deleteSafety2 = deleteSafety;

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, AudioLoader.name);

            if (!Directory.Exists(path))
            {
                if (GUILayout.Button(EditorTool.TryGetText("project_setting.audio.audios_folder_create"), GUILayout.ExpandWidth(false)))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                string jsonPath = path + ".json";
                if (!File.Exists(jsonPath))
                {
                    if (GUILayout.Button(EditorTool.TryGetText("project_setting.audio.audios_file_create"), GUILayout.ExpandWidth(false)))
                    {
                        File.WriteAllText(jsonPath, "{}");
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    bool isChanged = false;
                    List<KeyValuePair<string?, AudioData?>> audioDatas = JsonManager.JsonRead<Dictionary<string?, AudioData?>>(jsonPath).ToList();

                    //오디오 리스트
                    displayRestrictionsIndex = EditorTool.DrawRawList(audioDatas, "", x =>
                    {
                        KeyValuePair<string?, AudioData?> pair = (KeyValuePair<string?, AudioData?>)x;
                        EditorGUI.BeginChangeCheck();

                        string key;
                        string subtitle;
                        bool isBGM;

                        {
                            EditorTool.BeginLabelWidth(50);
                            EditorGUI.BeginChangeCheck();

                            key = EditorGUILayout.TextField(EditorTool.TryGetText("gui.key"), pair.Key);
                            subtitle = EditorGUILayout.TextField(EditorTool.TryGetText("gui.subtitle"), pair.Value?.subtitle);
                            isBGM = EditorGUILayout.Toggle("is BGM", pair.Value != null && pair.Value.isBGM);

                            isChanged = isChanged || EditorGUI.EndChangeCheck();
                            EditorTool.EndLabelWidth();
                        }

                        List<AudioMetaData>? metaDatas = null;
                        if (pair.Value != null)
                        {
                            metaDatas = pair.Value.audios.ToList();

                            //오디오 메타데이터 리스트
                            EditorTool.DrawRawList(metaDatas, "", y =>
                            {
                                AudioMetaData metaData = (AudioMetaData)y;
                                string audioPath = metaData.path;
                                bool stream = metaData.stream;
                                double pitch = metaData.pitch;
                                double tempo = metaData.tempo;
                                int loopStartIndex = metaData.loopStartIndex;
                                int loopOffsetIndex = metaData.loopOffsetIndex;

                                {
                                    EditorGUILayout.BeginHorizontal();
                                    string label = EditorTool.TryGetText("gui.path");

                                    EditorTool.BeginLabelWidth(label);
                                    EditorGUI.BeginChangeCheck();

                                    audioPath = EditorGUILayout.TextField(label, audioPath);

                                    isChanged = isChanged || EditorGUI.EndChangeCheck();
                                    EditorTool.EndLabelWidth();

                                    //GUI
                                    {
                                        string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace2, AudioLoader.name).Replace("\\", "/");
                                        string assetAllPathAndName = Path.Combine(assetAllPath, audioPath).Replace("\\", "/");

                                        string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace2, AudioLoader.name).Replace("\\", "/");
                                        string assetPathAndName = Path.Combine(assetPath, audioPath).Replace("\\", "/");

                                        ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ExtensionFilter.musicFileFilter);

                                        EditorGUI.BeginChangeCheck();

                                        DefaultAsset audioClip;
                                        if (audioPath != "")
                                        {
                                            audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false, GUILayout.Width(100));
                                        }
                                        else
                                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false, GUILayout.Width(100));

                                        isChanged = isChanged || EditorGUI.EndChangeCheck();

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
                                        string label = EditorTool.TryGetText("gui.pitch");
                                        EditorTool.BeginLabelWidth(label);

                                        pitch = EditorGUILayout.DoubleField(label, pitch);
                                        EditorTool.EndLabelWidth();
                                    }

                                    {
                                        string label = EditorTool.TryGetText("gui.tempo");
                                        EditorTool.BeginLabelWidth(label);

                                        tempo = EditorGUILayout.DoubleField(label, tempo);
                                        EditorTool.EndLabelWidth();
                                    }

                                    if (metaData.stream)
                                        tempo = tempo.Clamp(0);

                                    {
                                        string label = EditorTool.TryGetText("gui.stream");
                                        EditorTool.BeginLabelWidth(label);

                                        stream = EditorGUILayout.Toggle(label, stream, GUILayout.Width(EditorGUIUtility.labelWidth + 18));
                                        EditorTool.EndLabelWidth();
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();

                                    {
                                        string label = EditorTool.TryGetText("project_setting.audio.loop_start_index");
                                        EditorTool.BeginLabelWidth(label);

                                        loopStartIndex = EditorGUILayout.IntField(label, loopStartIndex).Clamp(0);
                                        EditorTool.EndLabelWidth();
                                    }

                                    {
                                        string label = EditorTool.TryGetText("project_setting.audio.loop_offset_index");
                                        EditorTool.BeginLabelWidth(label);

                                        loopOffsetIndex = EditorGUILayout.IntField(label, loopOffsetIndex).Clamp(0);
                                        EditorTool.EndLabelWidth();
                                    }

                                    isChanged = isChanged || EditorGUI.EndChangeCheck();
                                    EditorGUILayout.EndHorizontal();
                                }

                                return new AudioMetaData(audioPath, pitch, tempo, stream, loopStartIndex, loopOffsetIndex, null);
                            }, i => string.IsNullOrEmpty(metaDatas[i].path), i => metaDatas.Insert(i, new AudioMetaData("", 1, 1, false, 0, 0, null)), out bool isListChanged, deleteSafety2);

                            isChanged = isChanged || isListChanged;
                        }

                        return new KeyValuePair<string?, AudioData?>(key, new AudioData(subtitle, isBGM, metaDatas?.ToArray()));
                    }, i =>
                    {
                        KeyValuePair<string?, AudioData?> pair = audioDatas[i];
                        bool a = string.IsNullOrEmpty(pair.Key);
                        bool b = pair.Value == null || !pair.Value.audios.Any();

                        return a && b;
                    }, i => audioDatas.Add(new KeyValuePair<string?, AudioData?>("", new AudioData("", false))), out bool isListChanged, deleteSafety, 3, displayRestrictionsIndex);

                    isChanged = isChanged || isListChanged;

                    //키 중복 감지 및 리스트를 딕셔너리로 변환
                    if (isChanged)
                    {
                        bool overlap = audioDatas.Count != audioDatas.Distinct(new AudioDataEqualityComparer()).Count();
                        if (!overlap)
                            File.WriteAllText(jsonPath, JsonManager.ToJson(audioDatas.ToDictionary(x => x.Key ?? "", x => x.Value)));
                    }
                }
            }

            EditorTool.EndFieldWidth();
        }

        class AudioDataEqualityComparer : IEqualityComparer<KeyValuePair<string?, AudioData?>>
        {
            public bool Equals(KeyValuePair<string?, AudioData?> x, KeyValuePair<string?, AudioData?> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string?, AudioData?> obj) => obj.Key != null ? obj.Key.GetHashCode() : 0;
        }
    }
}
