#nullable enable
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [InitializeOnLoad]
    public abstract class TypeDrawer
    {
        static TypeDrawer()
        {
            typeDrawers.Clear();

            for (int i = 0; i < ReflectionManager.types.Count; i++)
            {
                Type type = ReflectionManager.types[i];
                if (!type.IsSubtypeOf<TypeDrawer>())
                    continue;

                typeDrawers.Add(type);
            }
        }

        public static List<Type> typeDrawers = new List<Type>();

        public TypeDrawer(Type type) => Recalculate(type);
        public TypeDrawer(object? instance) => Recalculate(instance);

        public abstract Type targetType { get; }

        public void Recalculate(Type type) => Recalculate(type, null);
        public void Recalculate(object? instance) => Recalculate(instance?.GetType(), instance);

        protected abstract void Recalculate(Type? type, object? instance);

        public virtual float GetPropertyHeight() => GetYSize(editorLabelStyle);

        public void OnGUI(Rect position, string? label) => OnGUI(position, new GUIContent(label));
        public virtual void OnGUI(Rect position, GUIContent? label) => EditorGUI.LabelField(position, label, "No GUI Implemented");
    }
}
