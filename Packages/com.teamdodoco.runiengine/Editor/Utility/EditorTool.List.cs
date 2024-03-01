#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public delegate object DrawRawListFunc(object value);
        public delegate bool DrawRawListDefaultValueFunc(int index);
        public delegate void DrawRawListAddRemoveFunc(int index);

        public static void DeleteSafety(ref bool value) => value = EditorGUILayout.Toggle(TryGetText("gui.delete_safety"), value);

        public static int DrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, out bool isListChanged, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(list, label, drawFunc, defaultValueFunc, addFunc, false, Vector2.zero, out isListChanged, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, Vector2 scrollViewPos, out bool isListChanged, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(list, label, drawFunc, defaultValueFunc, addFunc, true, scrollViewPos, out isListChanged, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) InternalDrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, bool scrollView, Vector2 scrollViewPos, out bool isListChanged, bool deleteSafety, int displayRestrictions, int displayRestrictionsIndex)
        {
            isListChanged = false;

            {
                EditorGUILayout.BeginVertical(otherHelpBox);

                //GUI
                {
                    BeginLabelWidth(100);

                    BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.label);
                    BeginAlignment(TextAnchor.MiddleLeft, EditorStyles.textField);

                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUI.BeginChangeCheck();

                            int count = EditorGUILayout.IntField(TryGetText("gui.list_count"), list.Count, GUILayout.Height(GetButtonYSize()));
                            //변수 설정
                            if (count < 0)
                                count = 0;

                            if (count > list.Count)
                            {
                                for (int i = list.Count; i < count; i++)
                                    addFunc.Invoke(list.Count);
                            }
                            else if (count < list.Count)
                            {
                                for (int i = list.Count - 1; i >= count; i--)
                                {
                                    if (!deleteSafety || list.Count > 0 && (list[list.Count - 1] == null || defaultValueFunc.Invoke(list.Count - 1)))
                                        list.RemoveAt(list.Count - 1);
                                    else
                                        count++;
                                }
                            }

                            isListChanged = isListChanged || EditorGUI.EndChangeCheck();
                        }

                        {
                            if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                            {
                                addFunc.Invoke(list.Count);
                                isListChanged = true;
                            }
                        }

                        {
                            EditorGUI.BeginDisabledGroup(deleteSafety && (list.Count <= 0 || list[list.Count - 1] != null && !defaultValueFunc.Invoke(list.Count - 1)));

                            if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)) && list.Count > 0)
                            {
                                list.RemoveAt(list.Count - 1);
                                isListChanged = true;
                            }

                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    {
                        EditorGUILayout.BeginHorizontal();

                        {
                            EditorGUI.BeginChangeCheck();

                            int count = EditorGUILayout.IntField(TryGetText("gui.screen_index"), displayRestrictionsIndex, GUILayout.Height(21));
                            //변수 설정
                            if (count < 0)
                                count = 0;

                            if (count > displayRestrictionsIndex)
                            {
                                for (int i = displayRestrictionsIndex; i < count && (list.Count - (displayRestrictions * displayRestrictionsIndex)) > displayRestrictions; i++)
                                    displayRestrictionsIndex++;
                            }
                            else if (count < displayRestrictionsIndex)
                            {
                                for (int i = displayRestrictionsIndex; i >= count; i--)
                                {
                                    if (displayRestrictionsIndex > 0)
                                        displayRestrictionsIndex--;
                                    else
                                        count++;
                                }
                            }

                            isListChanged = isListChanged || EditorGUI.EndChangeCheck();
                        }

                        {
                            EditorGUI.BeginDisabledGroup(displayRestrictionsIndex <= 0);

                            if (GUILayout.Button(TryGetText("gui.previously"), GUILayout.ExpandWidth(false)))
                                displayRestrictionsIndex--;

                            EditorGUI.EndDisabledGroup();
                        }

                        {
                            EditorGUI.BeginDisabledGroup((list.Count - (displayRestrictions * displayRestrictionsIndex)) <= displayRestrictions);

                            if (GUILayout.Button(TryGetText("gui.next"), GUILayout.ExpandWidth(false)))
                                displayRestrictionsIndex++;

                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EndAlignment(EditorStyles.textField);
                    EndAlignment(EditorStyles.label);

                    EndLabelWidth();
                }

                //List
                {
                    if (scrollView)
                        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

                    int loopCount = 0;
                    for (int i = displayRestrictions * displayRestrictionsIndex; i < list.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(otherHelpBox);
                        EditorGUILayout.BeginHorizontal();
                        
                        if (!string.IsNullOrEmpty(label))
                            GUILayout.Label(label, GUILayout.ExpandWidth(false));

                        EditorGUILayout.EndHorizontal();

                        if (drawFunc != null)
                            list[i] = drawFunc.Invoke(list[i]);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();

                        {
                            if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                            {
                                addFunc.Invoke(i + 1);
                                isListChanged = true;
                            }
                        }

                        {
                            EditorGUI.BeginDisabledGroup(deleteSafety && list[i] != null && !defaultValueFunc.Invoke(i));

                            if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                            {
                                list.RemoveAt(i);
                                isListChanged = true;
                            }

                            EditorGUI.EndDisabledGroup();
                        }

                        {
                            EditorGUI.BeginDisabledGroup(i - 1 < 0);

                            if (GUILayout.Button(TryGetText("gui.previously"), GUILayout.ExpandWidth(false)))
                            {
                                list.Move(i, i - 1);
                                isListChanged = true;
                            }

                            EditorGUI.EndDisabledGroup();
                        }

                        {
                            EditorGUI.BeginDisabledGroup(i + 1 >= list.Count);

                            if (GUILayout.Button(TryGetText("gui.next"), GUILayout.ExpandWidth(false)))
                            {
                                list.Move(i, i + 1);
                                isListChanged = true;
                            }

                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUILayout.EndHorizontal();

                        Space(3);

                        EditorGUILayout.EndVertical();

                        loopCount++;
                        if (loopCount >= displayRestrictions)
                            break;
                    }

                    if (scrollView)
                        EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();
            }

            return (scrollViewPos, displayRestrictionsIndex);
        }

        public static string DrawStringArray(string value, string[] array, params GUILayoutOption[] options) => InternalDrawStringArray("", value, array, false, out _, options);
        public static string DrawStringArray(string label, string value, string[] array, params GUILayoutOption[] options) => InternalDrawStringArray(label, value, array, true, out _, options);
        public static string DrawStringArray(string value, string[] array, out int index, params GUILayoutOption[] options) => InternalDrawStringArray("", value, array, false, out index, options);
        public static string DrawStringArray(string label, string value, string[] array, out int index, params GUILayoutOption[] options) => InternalDrawStringArray(label, value, array, true, out index, options);
        static string InternalDrawStringArray(string label, string value, string[] array, bool labelShow, out int index, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            if (labelShow)
                EditorGUILayout.PrefixLabel(label);

            value = EditorGUILayout.TextField(value);

            //원래 이딴거 안해도 루트 폴더 잘 감지했는데 tq 갑자기 안됨 유니티 병신
            List<string?> displayList = new List<string?>();
            List<int> indexList = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];

                bool startWith = false;
                for (int k = 0; k < array.Length; k++)
                {
                    if (path == Path.GetDirectoryName(array[k]).Replace("\\", "/"))
                    {
                        startWith = true;
                        break;
                    }
                }

                if (!startWith)
                {
                    displayList.Add(path);
                    indexList.Add(i);
                }

                if (path.Contains('/'))
                {
                    string parentPath = Path.GetDirectoryName(path).Replace("\\", "/");
                    if (!displayList.Contains(parentPath + "/root"))
                    {
                        displayList.Insert(displayList.Count - 1, parentPath + "/root");
                        displayList.Insert(displayList.Count - 1, parentPath + "/");

                        indexList.Insert(indexList.Count - 1, Array.IndexOf(array, parentPath));
                        indexList.Insert(indexList.Count - 1, int.MinValue);
                    }
                }
            }

            index = EditorGUILayout.IntPopup(Array.IndexOf(array, value), displayList.ToArray(), indexList.ToArray(), options);

            EditorGUILayout.EndHorizontal();

            if (index >= 0)
                return array[index];
            else
                return value;
        }

        class InternalStringArrayInfo
        {
            public string path = "";
            public List<InternalStringArrayInfo> stringArray = new List<InternalStringArrayInfo>();

            public InternalStringArrayInfo(string path) => this.path = path;
        }
    }
}
