#nullable enable
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public delegate object DrawRawListFunc(object value);
        public delegate bool DrawRawListDefaultValueFunc(int index);

        public static void DeleteSafety(ref bool value)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(TryGetText("gui.delete_safety"), GUILayout.Width(330));
            value = EditorGUILayout.Toggle(value);

            EditorGUILayout.EndHorizontal();
        }

        public static int DrawRawList<T>(IList list, string label, DrawRawListFunc topFunc, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(typeof(T), list, label, topFunc, false, Vector2.zero, out isChanged, tab, tab2, deleteSafety, (int index) => list[index].Equals(default(T)), displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList<T>(IList list, string label, DrawRawListFunc topFunc, Vector2 scrollViewPos, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(typeof(T), list, label, topFunc, true, scrollViewPos, out isChanged, tab, tab2, deleteSafety, (int index) => list[index].Equals(default(T)), displayRestrictions, displayRestrictionsIndex);
        public static int DrawRawList<T>(IList list, string label, DrawRawListFunc topFunc, DrawRawListDefaultValueFunc defaultValueFunc, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(typeof(T), list, label, topFunc, false, Vector2.zero, out isChanged, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList<T>(IList list, string label, DrawRawListFunc topFunc, DrawRawListDefaultValueFunc defaultValueFunc, Vector2 scrollViewPos, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(typeof(T), list, label, topFunc, true, scrollViewPos, out isChanged, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex);
        public static int DrawRawList(Type type, IList list, string label, DrawRawListFunc topFunc, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(type, list, label, topFunc, false, Vector2.zero, out isChanged, tab, tab2, deleteSafety, (int index) => list[index].Equals(type.GetDefaultValue()), displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList(Type type, IList list, string label, DrawRawListFunc topFunc, Vector2 scrollViewPos, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(type, list, label, topFunc, true, scrollViewPos, out isChanged, tab, tab2, deleteSafety, (int index) => list[index].Equals(type.GetDefaultValue()), displayRestrictions, displayRestrictionsIndex);
        public static int DrawRawList(Type type, IList list, string label, DrawRawListFunc topFunc, DrawRawListDefaultValueFunc defaultValueFunc, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(type, list, label, topFunc, false, Vector2.zero, out isChanged, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList(Type type, IList list, string label, DrawRawListFunc topFunc, DrawRawListDefaultValueFunc defaultValueFunc, Vector2 scrollViewPos, out bool isChanged, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(type, list, label, topFunc, true, scrollViewPos, out isChanged, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) InternalDrawRawList(Type type, IList list, string label, DrawRawListFunc topFunc, bool scrollView, Vector2 scrollViewPos, out bool isChanged, int tab, int tab2, bool deleteSafety, DrawRawListDefaultValueFunc defaultValueFunc, int displayRestrictions, int displayRestrictionsIndex)
        {
            isChanged = false;

            //GUI
            {
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab);

                {
                    EditorGUI.BeginChangeCheck();

                    int count = EditorGUILayout.IntField(TryGetText("gui.list_count"), list.Count, GUILayout.Height(21));
                    //변수 설정
                    if (count < 0)
                        count = 0;

                    if (count > list.Count)
                    {
                        for (int i = list.Count; i < count; i++)
                        {
                            /*if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                                ((IBeatValuePairList)list).Add();
                            else*/
                                list.Add(type.GetDefaultValueNotNull());
                        }
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

                    isChanged = isChanged || EditorGUI.EndChangeCheck();
                }

                {
                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        /*if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                            ((IBeatValuePairList)list).Add();
                        else*/
                            list.Add(type.GetDefaultValueNotNull());

                        isChanged = isChanged || true;
                    }
                }

                {
                    EditorGUI.BeginDisabledGroup(deleteSafety && (list.Count <= 0 || list[list.Count - 1] != null && !defaultValueFunc.Invoke(list.Count - 1)));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)) && list.Count > 0)
                    {
                        list.RemoveAt(list.Count - 1);
                        isChanged = isChanged || true;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab);

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

                    isChanged = isChanged || EditorGUI.EndChangeCheck();
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

            DrawLine();

            if (scrollView)
                scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

            int loopCount = 0;
            for (int i = displayRestrictions * displayRestrictionsIndex; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab + tab2);

                if (!string.IsNullOrEmpty(label))
                    GUILayout.Label(label, GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();

                if (topFunc != null)
                {
                    EditorGUI.BeginChangeCheck();
                    list[i] = topFunc.Invoke(list[i]);
                    isChanged = isChanged || EditorGUI.EndChangeCheck();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                {
                    if (GUILayout.Button(TryGetText("gui.add"), GUILayout.ExpandWidth(false)))
                    {
                        /*if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                            ((IBeatValuePairList)list).Insert(i + 1);
                        else*/
                            list.Insert(i + 1, type.GetDefaultValueNotNull());

                        isChanged = isChanged || true;
                    }
                }

                {
                    EditorGUI.BeginDisabledGroup(deleteSafety && list[i] != null && !defaultValueFunc.Invoke(i));

                    if (GUILayout.Button(TryGetText("gui.remove"), GUILayout.ExpandWidth(false)))
                    {
                        list.RemoveAt(i);
                        isChanged = isChanged || true;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                {
                    EditorGUI.BeginDisabledGroup(i - 1 < 0);

                    if (GUILayout.Button(TryGetText("gui.previously"), GUILayout.ExpandWidth(false)))
                    {
                        list.Move(i, i - 1);
                        isChanged = isChanged || true;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                {
                    EditorGUI.BeginDisabledGroup(i + 1 >= list.Count);

                    if (GUILayout.Button(TryGetText("gui.next"), GUILayout.ExpandWidth(false)))
                    {
                        list.Move(i, i + 1);
                        isChanged = isChanged || true;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
                loopCount++;

                if (loopCount >= displayRestrictions)
                    break;

                if (i < list.Count - 1)
                    DrawLine();
            }

            if (scrollView)
                EditorGUILayout.EndScrollView();

            return (scrollViewPos, displayRestrictionsIndex);
        }

        public static string DrawStringArray(string value, string[] array) => InternalDrawStringArray("", value, array, false, out _);
        public static string DrawStringArray(string label, string value, string[] array) => InternalDrawStringArray(label, value, array, true, out _);
        public static string DrawStringArray(string value, string[] array, out int index) => InternalDrawStringArray("", value, array, false, out index);
        public static string DrawStringArray(string label, string value, string[] array, out int index) => InternalDrawStringArray(label, value, array, true, out index);
        static string InternalDrawStringArray(string label, string value, string[] array, bool labelShow, out int index)
        {
            if (!labelShow)
                index = EditorGUILayout.Popup(Array.IndexOf(array, value), array, GUILayout.MinWidth(0));
            else
                index = EditorGUILayout.Popup(label, Array.IndexOf(array, value), array, GUILayout.MinWidth(0));

            if (index >= 0)
                return array[index];
            else
                return value;
        }
    }
}
