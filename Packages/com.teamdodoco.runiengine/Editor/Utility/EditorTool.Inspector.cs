#nullable enable
using RuniEngine.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static string DrawNameSpace(string nameSpace, params GUILayoutOption[] options) => DrawStringArray(nameSpace, ResourceManager.GetNameSpaces(), options);
        public static string DrawNameSpace(string label, string nameSpace, params GUILayoutOption[] options) => DrawStringArray(label, nameSpace, ResourceManager.GetNameSpaces(), options);



        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, null, options);
        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, string? label, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, label, options);
        static SerializedProperty? InternalUseProperty(SerializedObject serializedObject, string propertyName, string? label, params GUILayoutOption[] options)
        {
            GUIContent? guiContent = null;
            if (!string.IsNullOrEmpty(label))
                guiContent = new GUIContent { text = label };

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

                EditorGUILayout.PropertyField(tps, guiContent, options);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            return tps;
        }



        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out _, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out _, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out index, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, out int index, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out index, out _, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index, out bool usePropertyChanged, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out index, out usePropertyChanged, options);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, out int index, out bool usePropertyChanged, params GUILayoutOption[] options) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out index, out usePropertyChanged, options);
        static string InternalUsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, bool labelShow, out int index, out bool usePropertyChanged, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            if (labelShow)
                EditorGUILayout.PrefixLabel(label);

            EditorGUI.BeginChangeCheck();

            bool mixed = EditorGUI.showMixedValue;

            SerializedProperty? serializedProperty = UseProperty(serializedObject, propertyName, "", options);
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



        public static RectOffset RectOffsetField(string label, RectOffset value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.label, GUILayout.Width(EditorGUIUtility.labelWidth));

            TextAnchor alignment = EditorStyles.label.alignment;
            EditorStyles.label.alignment = TextAnchor.MiddleLeft;

            {
                GUILayout.BeginVertical();

                string leftLabel = "L";
                string rightLabel = "R";
                string topLabel = "T";
                string bottomLabel = "B";

                BeginLabelWidth(leftLabel, rightLabel, topLabel, bottomLabel);

                {
                    GUILayout.BeginHorizontal();

                    value.left = EditorGUILayout.FloatField(leftLabel, value.left);
                    value.right = EditorGUILayout.FloatField(rightLabel, value.right);

                    GUILayout.EndHorizontal();
                }

                {
                    GUILayout.BeginHorizontal();

                    value.top = EditorGUILayout.FloatField(topLabel, value.top);
                    value.bottom = EditorGUILayout.FloatField(bottomLabel, value.bottom);
                    
                    GUILayout.EndHorizontal();
                }

                EndLabelWidth();

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            EditorStyles.label.alignment = alignment;

            return value;
        }
    }
}
