#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using RuniEngine.Editor.APIBridge.UnityEditorInternal;
using RuniEngine.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

using EditorGUI = UnityEditor.EditorGUI;
using EditorGUIUtility = UnityEditor.EditorGUIUtility;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static string DrawNameSpace(string nameSpace, params GUILayoutOption[] options) => DrawStringArray(nameSpace, ResourceManager.GetNameSpaces(), options);
        public static string DrawNameSpace(string label, string nameSpace, params GUILayoutOption[] options) => DrawStringArray(label, nameSpace, ResourceManager.GetNameSpaces(), options);



        static readonly Dictionary<string, AnimBool> usePropertyAnimBoolList = new Dictionary<string, AnimBool>();

        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, null, true, options);
        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, string? label, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, label, false, options);
        static SerializedProperty? InternalUseProperty(SerializedObject serializedObject, string propertyName, string? label, bool defaultLabel, params GUILayoutOption[] options)
        {
            GUIContent? guiContent = new GUIContent();
            if (!string.IsNullOrEmpty(label))
                guiContent = new GUIContent(label);

            SerializedProperty? tps;

            try
            {
                tps = serializedObject.FindProperty(propertyName);
            }
            catch (Exception)
            {
                GUILayout.Label(TryGetText("inspector.property_none").Replace("{name}", propertyName));
                return null;
            }
            
            if (tps != null)
            {
                EditorGUI.BeginChangeCheck();

                if (tps.isArray)
                {
                    AnimBool animBool;
                    {
                        string key = ReorderableListWrapper.GetPropertyIdentifier(tps);
                        if (!usePropertyAnimBoolList.TryGetValue(key, out animBool))
                        {
                            animBool = new AnimBool(tps.isExpanded);
                            usePropertyAnimBoolList[key] = animBool;
                        }
                    }

                    animBool.target = tps.isExpanded;

                    if (tps.isExpanded || animBool.faded != 0)
                    {
                        bool isExpanded = tps.isExpanded;
                        tps.isExpanded = true;

                        float foldoutHeight = GetYSize(EditorStyles.foldoutHeader);
                        float height = EditorGUI.GetPropertyHeight(tps);

                        Rect position = EditorGUILayout.GetControlRect(false, foldoutHeight.Lerp(height, animBool.faded), options);
                        Rect clip = position;

                        clip.size += clip.position;
                        clip.position = Vector2.zero;

                        clip.width += 4;
                        clip.height = position.y + foldoutHeight.Lerp(height, animBool.faded);

                        InspectorWindow.RepaintAllInspectors();

                        {
                            GUI.BeginClip(clip);

                            if (defaultLabel)
                                EditorGUI.PropertyField(position, tps);
                            else
                                EditorGUI.PropertyField(position, tps, guiContent);

                            GUI.EndClip();
                        }

                        if (tps.isExpanded == false)
                            isExpanded = !isExpanded;

                        tps.isExpanded = isExpanded;
                    }
                    else
                    {
                        float foldoutHeight = GetYSize(EditorStyles.foldoutHeader);

                        bool isExpanded = tps.isExpanded;
                        tps.isExpanded = true;

                        Rect position = EditorGUILayout.GetControlRect(false, foldoutHeight, options);
                        tps.isExpanded = isExpanded;

                        if (defaultLabel)
                            EditorGUI.PropertyField(position, tps);
                        else
                            EditorGUI.PropertyField(position, tps, guiContent);
                    }
                }
                else
                {
                    if (defaultLabel)
                        EditorGUILayout.PropertyField(tps, options);
                    else
                        EditorGUILayout.PropertyField(tps, guiContent, options);
                }

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            return tps;
        }



        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, null, value, array, out _, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string? label, string value, string[] array, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, out _, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, null, value, array, out index, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string? label, string value, string[] array, out int index, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, out index, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index, out bool usePropertyChanged, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, null, value, array, out index, out usePropertyChanged, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string? label, string value, string[] array, out int index, out bool usePropertyChanged, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, out index, out usePropertyChanged, options);
        static string InternalUsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string? label, string value, string[] array, out int index, out bool usePropertyChanged, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.PrefixLabel(label);

            EditorGUI.BeginChangeCheck();

            bool mixed = EditorGUI.showMixedValue;

            SerializedProperty? serializedProperty = UseProperty(serializedObject, propertyName, null, options);
            usePropertyChanged = EditorGUI.EndChangeCheck();

            EditorGUI.showMixedValue = mixed;

            //원래 이딴거 안해도 루트 폴더 잘 감지했는데 tq 갑자기 안됨 유니티 병신
            List<string?> displayList = new List<string?>();
            List<int> indexList = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];

                bool startWith = false;
                for (int k = 0; k < array.Length; k++)
                {
                    if (path == Path.Combine(array[k], "..").Replace("\\", "/"))
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
                    string parentPath = Path.Combine(path, "..").Replace("\\", "/");
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

            if (!usePropertyChanged)
            {
                if (index >= 0)
                    return array[index];
                else
                    return value;
            }
            else
                return serializedProperty == null ? "" : (serializedProperty.stringValue ?? "");
        }



        public static string UsePropertyAndDrawNameSpace(SerializedObject serializedObject, string propertyName, string nameSpace, params GUILayoutOption[] options) => UsePropertyAndDrawNameSpace(serializedObject, propertyName, nameSpace, options);
        public static string UsePropertyAndDrawNameSpace(SerializedObject serializedObject, string propertyName, string label, string nameSpace, params GUILayoutOption[] options) => UsePropertyAndDrawStringArray(serializedObject, propertyName, label, nameSpace, ResourceManager.GetNameSpaces(), options);



        public static bool TargetsIsEquals<TTarget, TValue>(Func<TTarget, TValue> func, params TTarget?[]? targets)
        {
            if (targets == null || targets.Length <= 0)
                return true;

            TValue? parentValue = default;
            for (int i = 0; i < targets.Length; i++)
            {
                TTarget? target = targets[i];
                if (target == null)
                    continue;

                parentValue = func(target);
                break;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                TTarget? target = targets[i];
                if (target == null)
                    continue;

                TValue value = func(target);
                if (!Equals(parentValue, value))
                    return false;

                parentValue = value;
            }

            return true;
        }

        public static string TargetsToString<TTarget, TValue>(Func<TTarget, TValue> func, params TTarget?[]? targets)
        {
            if (targets == null || targets.Length <= 0)
                return "null";

            if (!TargetsIsEquals(func, targets))
                return "-";

            TValue? value = default;
            for (int i = 0; i < targets.Length; i++)
            {
                TTarget? target = targets[i];
                if (target == null)
                    continue;

                value = func(target);
                break;
            }

            if (value != null)
                return value.ToString();
            else
                return "null";
        }

        public static void TargetsSetValue<TTarget, TValue>(Func<TTarget, TValue> getValueFunc, Func<TTarget, TValue> drawGUIFunc, Action<TTarget, TValue> setValueFunc, params TTarget?[]? targets)
        {
            if (targets == null || targets.Length <= 0)
                return;

            TTarget? target = targets[0];
            for (int i = 0; i < targets.Length; i++)
            {
                TTarget? target2 = targets[i];
                if (target2 == null)
                    continue;

                target = target2;
                break;
            }

            if (target == null)
                return;

            EditorGUI.showMixedValue = !TargetsIsEquals(getValueFunc, targets);
            EditorGUI.BeginChangeCheck();

            TValue value = drawGUIFunc(target);

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    TTarget? target2 = targets[i];
                    if (target2 != null)
                        setValueFunc(target2, value);
                }
            }

            EditorGUI.showMixedValue = false;
        }



        public static RectOffset RectOffsetField(Rect position, string label, RectOffset value) => RectOffsetField(position, new GUIContent(label), value);

        static readonly GUIContent[] rectOffsetLabels = new GUIContent[] { new GUIContent("L"), new GUIContent("R"), new GUIContent("T"), new GUIContent("B") };
        static readonly float[] rectOffsetValues = new float[4];
        public static RectOffset RectOffsetField(Rect position, GUIContent label, RectOffset value)
        {
            rectOffsetValues[0] = value.left;
            rectOffsetValues[1] = value.right;
            rectOffsetValues[2] = value.top;

            rectOffsetValues[3] = value.bottom;
            EditorGUI.MultiFloatField(position, label, rectOffsetLabels, rectOffsetValues);

            value.left = rectOffsetValues[0];
            value.right = rectOffsetValues[1];
            value.top = rectOffsetValues[2];
            value.bottom = rectOffsetValues[3];

            return value;
        }

        public static RectOffset RectOffsetField(string label, RectOffset value) => RectOffsetField(new GUIContent(label), value);
        public static RectOffset RectOffsetField(GUIContent label, RectOffset value)
        {
            float height;
            if (EditorGUIUtility.wideMode)
                height = EditorGUIUtility.singleLineHeight;
            else
                height = EditorGUIUtility.singleLineHeight * 2 + 2;

            Rect position = EditorGUILayout.GetControlRect(true, height);
            return RectOffsetField(position, label, value);
        }

        public static bool IsChildrenIncluded(SerializedProperty prop) => prop.propertyType == SerializedPropertyType.Generic || prop.propertyType == SerializedPropertyType.Vector4;
    }
}
