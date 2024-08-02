#nullable enable
using System;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    public abstract class TypeDrawer
    {
        public float GetPropertyHeight(Type type, GUIContent label) => GetPropertyHeight(type, null, label);
        public float GetPropertyHeight(object? instance, GUIContent label) => GetPropertyHeight(instance?.GetType(), instance, label);

        public void OnGUI(Rect position, Type type, GUIContent label) => OnGUI(position, type, null, label);
        public void OnGUI(Rect position, object? instance, GUIContent label) => OnGUI(position, instance?.GetType(), instance, label);

        protected virtual float GetPropertyHeight(Type? type, object? instance, GUIContent label) => GetYSize(label, editorLabelStyle);
        protected virtual void OnGUI(Rect position, Type? type, object? instance, GUIContent label) => EditorGUI.LabelField(position, label, "No GUI Implemented");
    }
}
