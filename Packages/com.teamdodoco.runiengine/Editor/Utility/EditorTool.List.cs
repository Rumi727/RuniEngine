#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public delegate object DrawRawListFunc(object value);
        public delegate bool DrawRawListDefaultValueFunc(int index);
        public delegate void DrawRawListAddRemoveFunc(int index);

        public static void DeleteSafetyLayout(ref bool value) => value = EditorGUILayout.Toggle(TryGetText("gui.delete_safety"), value);
        public static void DeleteSafety(Rect position, ref bool value) => value = EditorGUI.Toggle(position, TryGetText("gui.delete_safety"), value);

        public static bool ListHeaderLayout(IList list, string label, bool foldout) => ListHeaderLayout(list, new GUIContent(label), foldout, null, null);
        public static bool ListHeaderLayout(IList list, GUIContent label, bool foldout) => ListHeaderLayout(list, label, foldout, null, null);
        public static bool ListHeaderLayout(IList list, string label, bool foldout, Action<int>? addAction, Action<int>? removeAction) => ListHeaderLayout(list, new GUIContent(label), foldout, addAction, removeAction);
        public static bool ListHeaderLayout(IList list, GUIContent label, bool foldout, Action<int>? addAction, Action<int>? removeAction) => ListHeader(EditorGUILayout.GetControlRect(false, GetYSize(EditorStyles.foldoutHeader)), list, label, foldout, addAction, removeAction);

        public static bool ListHeader(Rect position, IList list, string label, bool foldout) => ListHeader(position, list, new GUIContent(label), foldout, null, null);
        public static bool ListHeader(Rect position, IList list, GUIContent label, bool foldout) => ListHeader(position, list, label, foldout, null, null);
        public static bool ListHeader(Rect position, IList list, string label, bool foldout, Action<int>? addAction, Action<int>? removeAction) => ListHeader(position, list, new GUIContent(label), foldout, addAction, removeAction);
        public static bool ListHeader(Rect position, IList list, GUIContent label, bool foldout, Action<int>? addAction, Action<int>? removeAction)
        {
            {
                Rect headerPosition = position;
                headerPosition.width -= 48;

                foldout = EditorGUI.BeginFoldoutHeaderGroup(headerPosition, foldout, label);
                EditorGUI.EndFoldoutHeaderGroup();
            }

            {
                Rect countPosition = position;
                countPosition.x += countPosition.width - 48;
                countPosition.width = 48;

                int count = EditorGUI.DelayedIntField(countPosition, list.Count);
                int addCount = count - list.Count;
                if (addCount > 0)
                {
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = list.Count;
                        if (addAction != null)
                            addAction(index);
                        else
                        {
                            list.GetType().IsAssignableToGenericType(typeof(IList<>), out Type? resultType);
                            list.Add((resultType?.GetGenericArguments()[0].GetDefaultValueNotNull()) ?? new object());
                        }
                    }
                }
                else
                {
                    addCount = -addCount;
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = list.Count - 1;
                        if (removeAction != null)
                            removeAction(index);
                        else
                            list.RemoveAt(index);
                    }
                }
            }

            return foldout;
        }

        public static void ListHeaderLayout(SerializedProperty property, string label) => ListHeaderLayout(property, new GUIContent(label), null, null);
        public static void ListHeaderLayout(SerializedProperty property, GUIContent label) => ListHeaderLayout(property, label, null, null);
        public static void ListHeaderLayout(SerializedProperty property, string label, Action<int>? addAction, Action<int>? removeAction) => ListHeaderLayout(property, new GUIContent(label), addAction, removeAction);
        public static void ListHeaderLayout(SerializedProperty property, GUIContent label, Action<int>? addAction, Action<int>? removeAction)
        {
            float height;
            if (property.IsInArray())
                height = GetYSize(EditorStyles.foldout);
            else
                height = GetYSize(EditorStyles.foldoutHeader);

            ListHeader(EditorGUILayout.GetControlRect(false, height), property, label, addAction, removeAction);
        }

        public static void ListHeader(Rect position, SerializedProperty property, string label) => ListHeader(position, property, new GUIContent(label), null, null);
        public static void ListHeader(Rect position, SerializedProperty property, GUIContent label) => ListHeader(position, property, label, null, null);
        public static void ListHeader(Rect position, SerializedProperty property, string label, Action<int>? addAction, Action<int>? removeAction) => ListHeader(position, property, new GUIContent(label), addAction, removeAction);
        public static void ListHeader(Rect position, SerializedProperty property, GUIContent label, Action<int>? addAction, Action<int>? removeAction)
        {
            bool isInArray = property.IsInArray();

            {
                Rect headerPosition = position;
                headerPosition.width -= 48;

                EditorGUI.BeginProperty(headerPosition, label, property);
                EditorGUI.showMixedValue = false;

                if (!isInArray)
                {
                    property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerPosition, property.isExpanded, label);
                    EditorGUI.EndFoldoutHeaderGroup();
                }
                else
                    property.isExpanded = EditorGUI.Foldout(headerPosition, property.isExpanded, label, true);

                EditorGUI.EndProperty();
            }

            {
                Rect countPosition = position;
                countPosition.x += countPosition.width - 48;
                countPosition.width = 48;

                int count = EditorGUI.DelayedIntField(countPosition, property.arraySize);
                int addCount = count - property.arraySize;
                if (addCount > 0)
                {
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = property.arraySize;
                        if (addAction != null)
                            addAction(index);
                        else
                            property.InsertArrayElementAtIndex(index);
                    }
                }
                else
                {
                    addCount = -addCount;
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = property.arraySize - 1;
                        if (removeAction != null)
                            removeAction(index);
                        else
                            property.DeleteArrayElementAtIndex(index);
                    }
                }
            }
        }

        public static int DrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, out bool isListChanged, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(list, label, drawFunc, defaultValueFunc, addFunc, false, Vector2.zero, out isListChanged, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, Vector2 scrollViewPos, out bool isListChanged, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => InternalDrawRawList(list, label, drawFunc, defaultValueFunc, addFunc, true, scrollViewPos, out isListChanged, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) InternalDrawRawList(IList list, string label, DrawRawListFunc drawFunc, DrawRawListDefaultValueFunc defaultValueFunc, DrawRawListAddRemoveFunc addFunc, bool scrollView, Vector2 scrollViewPos, out bool isListChanged, bool deleteSafety, int displayRestrictions, int displayRestrictionsIndex)
        {
            isListChanged = false;

            {
                EditorGUILayout.BeginVertical(otherHelpBoxStyle);

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
                        EditorGUILayout.BeginVertical(otherHelpBoxStyle);
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

        public static string DrawStringArray(RuniAdvancedDropdown dropdown, string value, string[] array, bool isPath = false, params GUILayoutOption[] options) => InternalDrawStringArray(dropdown, string.Empty, value, array, false, isPath, out _, options);
        public static string DrawStringArray(RuniAdvancedDropdown dropdown, string label, string value, string[] array, bool isPath = false, params GUILayoutOption[] options) => InternalDrawStringArray(dropdown, label, value, array, true, isPath, out _, options);
        public static string DrawStringArray(RuniAdvancedDropdown dropdown, string value, string[] array, bool isPath, out int index, params GUILayoutOption[] options) => InternalDrawStringArray(dropdown, string.Empty, value, array, false, isPath, out index, options);
        public static string DrawStringArray(RuniAdvancedDropdown dropdown, string label, string value, string[] array, bool isPath, out int index, params GUILayoutOption[] options) => InternalDrawStringArray(dropdown, label, value, array, true, isPath, out index, options);
        static string InternalDrawStringArray(RuniAdvancedDropdown dropdown, string label, string value, string[] array, bool labelShow, bool isPath, out int index, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            if (labelShow)
                EditorGUILayout.PrefixLabel(label);

            value = EditorGUILayout.TextField(value, options);

            /*//원래 이딴거 안해도 루트 폴더 잘 감지했는데 tq 갑자기 안됨 유니티 병신
            List<string?> displayList = new List<string?>();
            List<int> indexList = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];

                bool startWith = false;
                for (int k = 0; k < array.Length; k++)
                {
                    if (path == PathUtility.GetParentPath(array[k]).UniformDirectorySeparatorCharacter())
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
                    string parentPath = PathUtility.GetParentPath(path).UniformDirectorySeparatorCharacter();
                    if (!displayList.Contains(parentPath + "/root"))
                    {
                        int index2 = (displayList.Count - 1).Clamp(0);
                        displayList.Insert(index2, parentPath + "/root");
                        displayList.Insert(index2 + 1, parentPath + "/");

                        indexList.Insert(index2, Array.IndexOf(array, parentPath));
                        indexList.Insert(index2 + 1, int.MinValue);
                    }
                }
            }

            index = EditorGUILayout.IntPopup(Array.IndexOf(array, value), displayList.ToArray(), indexList.ToArray(), options);*/

            if (isPath)
            {
                value = dropdown.DrawLayoutPath(value, array, options);
                index = Array.IndexOf(array, value);
            }
            else
            {
                index = dropdown.DrawLayout(Array.IndexOf(array, value), array, options);
                if (index >= 0 && index < array.Length)
                    value = array[index];
            }

            EditorGUILayout.EndHorizontal();
            return value;
        }

        /// <summary><see cref="IList"/> 인터페이스의 리스트 타입을 가져옵니다</summary>
        public static Type GetListType(IList list) => GetListType(list.GetType());

        /// <summary><see cref="IList"/> 인터페이스의 리스트 타입을 가져옵니다</summary>
        public static Type GetListType(Type type)
        {
            type.IsAssignableToGenericType(typeof(IList<>), out Type? resultType);
            return resultType?.GetGenericArguments()[0] ?? typeof(object);
        }
    }
}
