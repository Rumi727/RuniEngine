#nullable enable
using RuniEngine.Resource;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static string DrawNameSpace(string nameSpace, params GUILayoutOption[] options) => DrawStringArray(nameSpace, ResourceManager.GetNameSpaces(), options);
        public static string DrawNameSpace(string label, string nameSpace, params GUILayoutOption[] options) => DrawStringArray(label, nameSpace, ResourceManager.GetNameSpaces(), options);



        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, "", false, options);
        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, string label, params GUILayoutOption[] options) => InternalUseProperty(serializedObject, propertyName, label, true, options);
        static SerializedProperty? InternalUseProperty(SerializedObject serializedObject, string propertyName, string label, bool labelShow, params GUILayoutOption[] options)
        {
            GUIContent? guiContent = null;
            if (labelShow)
                guiContent = new GUIContent { text = label };

            SerializedProperty? tps = null;

            try
            {
                tps = serializedObject.FindProperty(propertyName);
            }
            catch (ExitGUIException)
            {

            }
            catch (Exception)
            {
                GUILayout.Label(TryGetText("inspector.property_none").Replace("{name}", propertyName));
                return null;
            }

            if (tps != null)
            {
                EditorGUI.BeginChangeCheck();

                if (!labelShow)
                    EditorGUILayout.PropertyField(tps, true, options);
                else
                    EditorGUILayout.PropertyField(tps, guiContent, true, options);

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

            index = EditorGUILayout.Popup(Array.IndexOf(array, value), array, options);

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

        public static void BeginLabelWidth(string label) => BeginLabelWidth(new GUIContent(label));
        public static void BeginLabelWidth(GUIContent label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(string label, GUIStyle style) => BeginLabelWidth(new GUIContent(label), style);
        public static void BeginLabelWidth(GUIContent label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);

        public static void BeginLabelWidth(params string[] label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(params GUIContent[] label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(string[] label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);
        public static void BeginLabelWidth(GUIContent[] label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);

        static readonly Stack<float> labelWidthQueue = new Stack<float>();
        public static void BeginLabelWidth(float width)
        {
            labelWidthQueue.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = width;
        }

        public static void EndLabelWidth()
        {
            if (labelWidthQueue.TryPop(out float result))
                EditorGUIUtility.labelWidth = result;
            else
                EditorGUIUtility.labelWidth = 0;
        }

        public static float GetLabelXSize(string label) => GetLabelXSize(new GUIContent(label));
        public static float GetLabelXSize(GUIContent label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(string label, GUIStyle style) => GetLabelXSize(new GUIContent(label), style);
        public static float GetLabelXSize(GUIContent label, GUIStyle style) => style.CalcSize(label).x;

        public static float GetLabelXSize(params string[] label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(params GUIContent[] label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(string[] label, GUIStyle style)
        {
            float width = 0;
            for (int i = 0; i < label.Length; i++)
                width = width.Max(style.CalcSize(new GUIContent(label[i])).x);

            return width;
        }

        public static float GetLabelXSize(GUIContent[] label, GUIStyle style)
        {
            float width = 0;
            for (int i = 0; i < label.Length; i++)
                width = width.Max(style.CalcSize(label[i]).x);

            return width;
        }

        static readonly Stack<float> fieldWidthQueue = new Stack<float>();
        public static void BeginFieldWidth(float width)
        {
            fieldWidthQueue.Push(EditorGUIUtility.fieldWidth);
            EditorGUIUtility.fieldWidth = width;
        }

        public static void EndFieldWidth()
        {
            if (fieldWidthQueue.TryPop(out float result))
                EditorGUIUtility.fieldWidth = result;
            else
                EditorGUIUtility.fieldWidth = 0;
        }



        public static RectOffset RectOffsetField(string label, RectOffset value)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));

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

                    value.left = EditorGUILayout.IntField(leftLabel, value.left);
                    value.right = EditorGUILayout.IntField(rightLabel, value.right);

                    GUILayout.EndHorizontal();
                }

                {
                    GUILayout.BeginHorizontal();

                    value.top = EditorGUILayout.IntField(topLabel, value.top);
                    value.bottom = EditorGUILayout.IntField(bottomLabel, value.bottom);
                    
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
