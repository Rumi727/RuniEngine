#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    public abstract class FieldTypeDrawer : TypeDrawer
    {
        protected FieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public abstract override float GetPropertyHeight();

        protected override void InternalOnGUI(Rect position, GUIContent? label)
        {
            object? value;
            if (property.canRead)
            {
                value = property.GetValue();
                EditorGUI.showMixedValue = property.isMixed;

                if (value == null && property.IsNotNullField() && !property.isUnityObject)
                {
                    value = property.propertyType.GetDefaultValueNotNull();
                    property.SetValue(value);
                }
            }
            else
            {
                value = property.propertyType.GetDefaultValue();
                EditorGUI.showMixedValue = false;
            }
            
            if (property.IsNullableType())
            {
                if (property.DrawNullableButton(position, label, out bool isDrawed))
                    return;

                if (isDrawed)
                    position.width -= 42;
            }

            EditorGUI.BeginDisabledGroup(!property.canWrite);

            EditorGUI.BeginChangeCheck();
            value = DrawField(position, label, value);
            if (EditorGUI.EndChangeCheck())
                property.SetValue(value);

            EditorGUI.showMixedValue = false;
            EditorGUI.EndDisabledGroup();
        }

        public abstract object? DrawField(Rect position, GUIContent? label, object? value);
    }
}
