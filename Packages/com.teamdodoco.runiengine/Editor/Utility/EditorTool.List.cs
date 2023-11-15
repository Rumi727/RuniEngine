#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void DeleteSafety(ref bool value)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(TryGetText("gui.delete_safety"), GUILayout.Width(330));
            value = EditorGUILayout.Toggle(value);

            EditorGUILayout.EndHorizontal();
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
