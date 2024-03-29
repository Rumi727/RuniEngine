#nullable enable
/*
 * 이 스크립트는 SC KRM에서 따왔으며, 완벽한 리스트 대체제가 생긴다면 코드를 완전히 갈아엎어야합니다
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using RuniEngine.Pooling;
using RuniEngine.Datas;
using UnityEditorInternal;
using System;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class ObjectPoolingProjectSetting : SettingsProvider
    {
        public ObjectPoolingProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new ObjectPoolingProjectSetting("Runi Engine/Object Pooling", SettingsScope.Project);



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                objectPoolingProjectSetting ??= new StorableClass(typeof(ObjectPoolingManager.ProjectData));
                objectPoolingProjectSetting.AutoNameLoad(Kernel.projectDataPath);
            }

            ObjectPoolingManager.ProjectData.prefabList ??= new Dictionary<string, string>();
        }

        ReorderableList? reorderableList;
        public override void OnGUI(string searchContext) => DrawGUI(ref reorderableList);

        public static StorableClass? objectPoolingProjectSetting = null;
        public static void DrawGUI(ref ReorderableList? reorderableList)
        {
            List<KeyValuePair<string, string>> prefabObject = ObjectPoolingManager.ProjectData.prefabList.ToList();
            float height = EditorStyles.textField.CalcSize(new GUIContent()).y;
            bool isChanged = false;

            reorderableList ??= new ReorderableList(null, typeof(KeyValuePair<string, string>), true, true, true, true)
            {
                drawHeaderCallback = static (Rect rect) => GUI.Label(rect, TryGetText("project_setting.object_pooling.name")),
                elementHeight = (height + 8f) * 2,
                multiSelect = true
            };

            reorderableList.list = prefabObject;

            reorderableList.onAddCallback = x =>
            {
                prefabObject.Add(new KeyValuePair<string, string>("", ""));
                isChanged = true;
            };

            reorderableList.onRemoveCallback = x =>
            {
                if (x.selectedIndices.Count > 0)
                {
                    for (int i = 0; i < x.selectedIndices.Count; i++)
                    {
                        int index = x.selectedIndices[i];
                        prefabObject.RemoveAt(index.Clamp(0, x.list.Count - 1));
                    }
                }
                else
                    prefabObject.RemoveAt(x.list.Count - 1);

                isChanged = true;
            };

            reorderableList.onReorderCallback =
                x => isChanged = true;

            reorderableList.onCanAddCallback =
                x => !prefabObject.Contains(new KeyValuePair<string, string>("", ""), new PrefabObjectEqualityComparer());

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;

                if (Event.current.type == EventType.Repaint)
                    helpBox.Draw(new Rect(rect.x, rect.y, rect.width, rect.height - 4), new GUIContent(), 0);

                rect.x += 4;
                rect.y += 5;
                rect.width -= 12;

                KeyValuePair<string, string> item = prefabObject[index];
                float initX = rect.x;
                float initY = rect.y;
                float initWidth = rect.width;
                

                string key;
                {
                    string label = TryGetText("gui.prefab_key");

                    BeginLabelWidth(label);
                    EditorGUI.BeginChangeCheck();

                    key = EditorGUI.TextField(new Rect(rect.x, rect.y, initWidth, height), label, item.Key);

                    if (EditorGUI.EndChangeCheck())
                        isChanged = true;

                    EndLabelWidth();
                }

                rect.x = initX;
                rect.y += height + 4;

                /*
                 * PrefabObject의 <string, string>를 <string, GameObject>로 바꿔서 인스펙터에 보여주고 인스펙터에서 변경한걸 <string, string>로 다시 바꿔서 PrefabObject에 저장
                 * 
                 * 왜 이렇게 변환하냐면 JSON에 오브젝트를 저장할려면 우선적으로 string 값같은 경로가 있어야하고
                 * 인스펙터에서 쉽게 드래그로 오브젝트를 바꾸기 위해선
                 * GameObject 형식이여야해서 이런 소용돌이가 나오게 된것
                */
                {
                    Vector2 size;

                    size.x = initWidth;
                    size.x *= 0.6f;
                    size.x -= 1.5f;

                    {
                        string label = TryGetText("gui.prefab");
                        BeginLabelWidth(label);

                        EditorGUI.BeginChangeCheck();
                        string assetsPath = EditorGUI.TextField(new Rect(rect.x, rect.y, size.x, height), label, item.Value);

                        if (EditorGUI.EndChangeCheck())
                        {
                            prefabObject[index] = new KeyValuePair<string, string>(key, assetsPath);
                            isChanged = true;
                        }
                    }

                    EndLabelWidth();

                    rect.x += size.x + 3;
                    size.x = initWidth;
                    size.x *= 0.4f;
                    size.x -= 1.5f;

                    //문자열(경로)을 프리팹으로 변환
                    {
                        EditorGUI.BeginChangeCheck();
                        GameObject? gameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.x, rect.y, size.x, height), Resources.Load<GameObject>(item.Value), typeof(GameObject), false);

                        if (EditorGUI.EndChangeCheck() && gameObject != null)
                        {
                            string assetsPath = AssetDatabase.GetAssetPath(gameObject);
                            if (assetsPath.Contains("Resources/"))
                            {
                                assetsPath = assetsPath.Substring(assetsPath.IndexOf("Resources/") + 10);
                                assetsPath = assetsPath.Remove(assetsPath.LastIndexOf("."));

                                prefabObject[index] = new KeyValuePair<string, string>(key, assetsPath);
                            }
                            else
                                prefabObject[index] = new KeyValuePair<string, string>(key, string.Empty);

                            isChanged = true;
                        }
                    }
                }
            };

            EditorGUILayout.HelpBox(TryGetText("project_setting.object_pooling.warning"), MessageType.Info);

            Rect rect = EditorGUILayout.GetControlRect(true, reorderableList.GetHeight());
            reorderableList.DoList(rect);

            //키 중복 감지 및 리스트를 딕셔너리로 변환
            bool overlap = prefabObject.Count != prefabObject.Distinct(new PrefabObjectEqualityComparer()).Count();
            if (!overlap)
                ObjectPoolingManager.ProjectData.prefabList = prefabObject.ToDictionary(x => x.Key, x => x.Value);

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (isChanged && !Kernel.isPlaying && objectPoolingProjectSetting != null)
                objectPoolingProjectSetting.AutoNameSave(Kernel.projectDataPath);
        }

        class PrefabObjectEqualityComparer : IEqualityComparer<KeyValuePair<string, string>>
        {
            public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y) => x.Key == y.Key;
            public int GetHashCode(KeyValuePair<string, string> obj) => obj.Key.GetHashCode();
        }
    }
}
