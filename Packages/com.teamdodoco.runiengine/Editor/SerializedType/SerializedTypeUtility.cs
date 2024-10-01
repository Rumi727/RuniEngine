#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.SerializedTypes
{
    public static class SerializedTypeUtility
    {
        public static float GetPropertyHeight(this SerializedTypeProperty property) => property.typeDrawer.GetPropertyHeight();

        public static void DrawGUILayout(this SerializedTypeProperty property, string? label) => DrawGUILayout(property, new GUIContent(label));
        public static void DrawGUILayout(this SerializedTypeProperty property, GUIContent? label)
        {
            Rect position = EditorGUILayout.GetControlRect(false, property.GetPropertyHeight());
            property.typeDrawer.OnGUI(position, label);
        }

        public static void DrawGUI(this SerializedTypeProperty property, Rect position, string? label) => property.typeDrawer.OnGUI(position, label);
        public static void DrawGUI(this SerializedTypeProperty property, Rect position, GUIContent? label) => property.typeDrawer.OnGUI(position, label);

        public static bool IsChildrenIncluded(this Type type) => !type.IsPrimitive && type != typeof(string);

        public static bool IsNullableType(this SerializedTypeProperty property) => IsNullableType(property.realPropertyType);
        public static bool IsNullableType(this Type type) => type.IsClass || type.IsNullableValueType();

        public static bool IsNullableValueType(this SerializedTypeProperty property) => IsNullableValueType(property.realPropertyType);
        public static bool IsNullableValueType(this Type type) => type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool IsNotNullField(this SerializedTypeProperty property) => property.AttributeContains<NotNullFieldAttribute>();

        public static object CreateNullableInstance(this SerializedTypeProperty property)
        {
            if (property.propertyType == typeof(string))
                return string.Empty;

            object value;
            if (property.IsNullableValueType())
                value = Activator.CreateInstance(property.realPropertyType, Activator.CreateInstance(property.propertyType));
            else
                value = Activator.CreateInstance(property.propertyType);

            return value;
        }

        /// <summary>Null 값이면 true 반환</summary>
        public static bool DrawNullableButton(this SerializedTypeProperty property, Rect position, GUIContent? label, out bool isDrawed)
        {
            if (!property.IsNullableType() || property.IsNotNullField() || property.isUnityObject)
            {
                isDrawed = false;
                return false;
            }

            isDrawed = true;

            Rect nullPosition = position;
            if (property.canRead)
            {
                nullPosition.x += nullPosition.width - 40;
                nullPosition.width = 40;

                bool isNull = property.GetValue() == null;
                if (isNull)
                    EditorGUI.LabelField(position, new GUIContent($"{label} ({property.propertyType.Name})"), new GUIContent("null"));

                EditorGUI.BeginDisabledGroup(!property.canWrite);

                if (isNull)
                {
                    if (GUI.Button(nullPosition, "+"))
                        property.SetValue(property.CreateNullableInstance());
                }
                else if (GUI.Button(nullPosition, "-"))
                {
                    property.SetValue(null);
                    isNull = true;
                }

                EditorGUI.EndDisabledGroup();

                return isNull;
            }
            else if (property.canWrite)
            {
                nullPosition.x += nullPosition.width - 40;
                nullPosition.width = 19;

                if (GUI.Button(nullPosition, "+"))
                    property.SetValue(property.CreateNullableInstance());

                nullPosition.x += 21;

                if (GUI.Button(nullPosition, "-"))
                    property.SetValue(null);

                return false;
            }

            return false;
        }
    }
}
