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
    public class NBSProjectSetting : SettingsProvider
    {
        public NBSProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new NBSProjectSetting("Runi Engine/NBS Setting", SettingsScope.Project);



        string nameSpace = "";
        bool deleteSafety = true;
        int displayRestrictionsIndex = 0;
        public override void OnGUI(string searchContext) => DrawGUI(ref nameSpace, ref deleteSafety, ref displayRestrictionsIndex);

        public static void DrawGUI(ref string nameSpace, ref bool deleteSafety, ref int displayRestrictionsIndex)
        {
            //왜 그런진 모르겠는데 이렇게 라벨 길이 설정 안해주면 클릭 인식 지랄남
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 150;

            EditorTool.DeleteSafety(ref deleteSafety);

            nameSpace = EditorTool.DrawNameSpace(EditorTool.TryGetText("gui.namespace"), nameSpace);
            if (string.IsNullOrEmpty(nameSpace))
                return;

            string nameSpace2 = nameSpace;
            bool deleteSafety2 = deleteSafety;

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, NBSLoader.name);

            if (!Directory.Exists(path))
            {
                if (GUILayout.Button(EditorTool.TryGetText("project_setting.nbs.nbses_folder_create"), GUILayout.ExpandWidth(false)))
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
                    if (GUILayout.Button(EditorTool.TryGetText("project_setting.nbs.nbses_file_create"), GUILayout.ExpandWidth(false)))
                    {
                        File.WriteAllText(jsonPath, "{}");
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    bool isChanged = false;
                    List<KeyValuePair<string?, NBSData?>> nbsDatas = JsonManager.JsonRead<Dictionary<string?, NBSData?>>(jsonPath).ToList();

                    //오디오 리스트
                    displayRestrictionsIndex = EditorTool.DrawRawList(nbsDatas, "", x =>
                    {
                        KeyValuePair<string?, NBSData?> pair = (KeyValuePair<string?, NBSData?>)x;
                        EditorGUI.BeginChangeCheck();

                        string key = EditorGUILayout.TextField(EditorTool.TryGetText("gui.key"), pair.Key);
                        string subtitle = EditorGUILayout.TextField(EditorTool.TryGetText("gui.subtitle"), pair.Value?.subtitle);
                        bool isBGM = EditorGUILayout.Toggle("is BGM", pair.Value != null && pair.Value.isBGM);

                        isChanged = isChanged || EditorGUI.EndChangeCheck();

                        List<NBSMetaData>? metaDatas = null;
                        if (pair.Value != null)
                        {
                            metaDatas = pair.Value.nbses.ToList();

                            //오디오 메타데이터 리스트
                            EditorTool.DrawRawList(metaDatas, "", y =>
                            {
                                NBSMetaData metaData = (NBSMetaData)y;
                                string nbsPath = metaData.path;
                                bool stream = metaData.stream;
                                double pitch = metaData.pitch;
                                double tempo = metaData.tempo;

                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Label(EditorTool.TryGetText("gui.path"), GUILayout.ExpandWidth(false));


                                    EditorGUI.BeginChangeCheck();
                                    nbsPath = EditorGUILayout.TextField(nbsPath);
                                    isChanged = isChanged || EditorGUI.EndChangeCheck();

                                    //GUI
                                    {
                                        string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace2, NBSLoader.name).Replace("\\", "/");
                                        string assetAllPathAndName = Path.Combine(assetAllPath, nbsPath).Replace("\\", "/");

                                        string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace2, NBSLoader.name).Replace("\\", "/");
                                        string assetPathAndName = Path.Combine(assetPath, nbsPath).Replace("\\", "/");

                                        ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ExtensionFilter.nbsFileFilter);

                                        EditorGUI.BeginChangeCheck();

                                        DefaultAsset nbsClip;
                                        if (nbsPath != "")
                                        {
                                            nbsClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                            nbsClip = (DefaultAsset)EditorGUILayout.ObjectField(nbsClip, typeof(DefaultAsset), false);
                                        }
                                        else
                                            nbsClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false);

                                        isChanged = isChanged || EditorGUI.EndChangeCheck();

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

                                    GUILayout.Label(EditorTool.TryGetText("gui.pitch"), GUILayout.ExpandWidth(false));
                                    pitch = EditorGUILayout.DoubleField(pitch);

                                    GUILayout.Label(EditorTool.TryGetText("gui.tempo"), GUILayout.ExpandWidth(false));
                                    tempo = EditorGUILayout.DoubleField(tempo);

                                    if (metaData.stream)
                                        tempo = tempo.Clamp(0);

                                    GUILayout.Label(EditorTool.TryGetText("gui.stream"), GUILayout.ExpandWidth(false));
                                    stream = EditorGUILayout.Toggle(stream, GUILayout.Width(20));

                                    EditorGUILayout.EndHorizontal();
                                    /*EditorGUILayout.BeginHorizontal();

                                    GUILayout.Label(EditorTool.TryGetText("project_setting.nbs.loop_offset_tick"), GUILayout.ExpandWidth(false));
                                    loopOffsetTick = EditorGUILayout.IntField(loopOffsetTick).Clamp(0);

                                    isChanged = isChanged || EditorGUI.EndChangeCheck();
                                    EditorGUILayout.EndHorizontal();*/
                                }

                                return new NBSMetaData(nbsPath, pitch, tempo, stream, null);
                            }, i => string.IsNullOrEmpty(metaDatas[i].path), i => metaDatas.Insert(i, new NBSMetaData("", 1, 1, false, null)), out bool isListChanged, deleteSafety2);

                            isChanged = isChanged || isListChanged;
                        }

                        return new KeyValuePair<string?, NBSData?>(key, new NBSData(subtitle, isBGM, metaDatas?.ToArray()));
                    }, i =>
                    {
                        KeyValuePair<string?, NBSData?> pair = nbsDatas[i];
                        bool a = string.IsNullOrEmpty(pair.Key);
                        bool b = pair.Value == null || !pair.Value.nbses.Any();

                        return a && b;
                    }, i => nbsDatas.Add(new KeyValuePair<string?, NBSData?>("", new NBSData("", false))), out bool isListChanged, deleteSafety, 3, displayRestrictionsIndex);

                    isChanged = isChanged || isListChanged;

                    //키 중복 감지 및 리스트를 딕셔너리로 변환
                    if (isChanged)
                    {
                        bool overlap = nbsDatas.Count != nbsDatas.Distinct(new NBSDataEqualityComparer()).Count();
                        if (!overlap)
                            File.WriteAllText(jsonPath, JsonManager.ToJson(nbsDatas.ToDictionary(x => x.Key ?? "", x => x.Value)));
                    }
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;
        }

        class NBSDataEqualityComparer : IEqualityComparer<KeyValuePair<string?, NBSData?>>
        {
            public bool Equals(KeyValuePair<string?, NBSData?> x, KeyValuePair<string?, NBSData?> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string?, NBSData?> obj) => obj.Key != null ? obj.Key.GetHashCode() : 0;
        }
    }
}
