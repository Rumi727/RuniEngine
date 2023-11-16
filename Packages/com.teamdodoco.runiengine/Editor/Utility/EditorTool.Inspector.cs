#nullable enable
using RuniEngine.Resource;
using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static string DrawNameSpace(string nameSpace) => DrawStringArray(nameSpace, ResourceManager.GetNameSpaces());
        public static string DrawNameSpace(string label, string nameSpace) => DrawStringArray(label, nameSpace, ResourceManager.GetNameSpaces());



        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName) => InternalUseProperty(serializedObject, propertyName, "", false);
        public static SerializedProperty? UseProperty(SerializedObject serializedObject, string propertyName, string label) => InternalUseProperty(serializedObject, propertyName, label, true);
        static SerializedProperty? InternalUseProperty(SerializedObject serializedObject, string propertyName, string label, bool labelShow)
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
                    EditorGUILayout.PropertyField(tps, true);
                else
                    EditorGUILayout.PropertyField(tps, guiContent, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            return tps;
        }



        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out _, out _);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out _, out _);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out index, out _);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, out int index) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out index, out _);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string value, string[] array, out int index, out bool usePropertyChanged) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, "", value, array, false, out index, out usePropertyChanged);
        public static string UsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, out int index, out bool usePropertyChanged) => InternalUsePropertyAndDrawStringArray(serializedObject, propertyName, label, value, array, true, out index, out usePropertyChanged);
        static string InternalUsePropertyAndDrawStringArray(SerializedObject serializedObject, string propertyName, string label, string value, string[] array, bool labelShow, out int index, out bool usePropertyChanged)
        {
            EditorGUILayout.BeginHorizontal();

            if (labelShow)
                EditorGUILayout.PrefixLabel(label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty? serializedProperty = UseProperty(serializedObject, propertyName, "");
            usePropertyChanged = EditorGUI.EndChangeCheck();

            index = EditorGUILayout.Popup(Array.IndexOf(array, value), array, GUILayout.MinWidth(0));

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



        public static string UsePropertyAndDrawNameSpace(SerializedObject serializedObject, string propertyName, string nameSpace) => UsePropertyAndDrawNameSpace(serializedObject, propertyName, nameSpace);
        public static string UsePropertyAndDrawNameSpace(SerializedObject serializedObject, string propertyName, string label, string nameSpace) => UsePropertyAndDrawStringArray(serializedObject, propertyName, label, nameSpace, ResourceManager.GetNameSpaces());



        public static bool TargetsIsEquals<TTarget, TValue>(Func<TTarget, TValue> func, params TTarget?[]? targets)
        {
            if (targets == null || targets.Length <= 0)
                return true;

            TValue? parentValue = default;
            for (int i = 1; i < targets.Length; i++)
            {
                TTarget? target = targets[i];
                if (target == null)
                    continue;

                parentValue = func(target);
                break;
            }

            for (int i = 1; i < targets.Length; i++)
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
            for (int i = 1; i < targets.Length; i++)
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
    }
}
