#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Attribute
{
    [CustomPropertyDrawer(typeof(FieldNameAttribute), true)]
    public sealed class FieldNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldNameAttribute attribute = (FieldNameAttribute)this.attribute;
            label.text = EditorTool.TryGetText(attribute.name);

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
