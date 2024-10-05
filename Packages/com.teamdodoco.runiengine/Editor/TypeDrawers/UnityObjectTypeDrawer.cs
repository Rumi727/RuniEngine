#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(Object))]
    public class UnityObjectTypeDrawer : FieldTypeDrawer
    {
        protected UnityObjectTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override float InternalGetPropertyHeight() => GetYSize(EditorStyles.objectField);
        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            if (value == null && property.IsNotNullField())
            {
                position.width *= 0.6666666666666666666666666666666666666f;
                position.width -= 4;

                value = EditorGUI.ObjectField(position, label, (Object?)value, property.propertyType, true);

                position.x += position.width + 4;
                position.width *= 0.5f;
                position.width += 2;

                EditorGUI.HelpBox(position, TryGetText("gui.field_is_null"), MessageType.Error);

                return value;
            }
            else
                return EditorGUI.ObjectField(position, label, (Object?)value, property.propertyType, true);
        }
    }
}
