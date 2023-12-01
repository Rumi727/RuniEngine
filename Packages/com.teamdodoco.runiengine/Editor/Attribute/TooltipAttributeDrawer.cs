#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Attribute
{
    [CustomPropertyDrawer(typeof(TooltipAttribute), true)]
    public sealed class TooltipAttributeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TooltipAttribute attribute = (TooltipAttribute)this.attribute;
            label.tooltip = EditorTool.TryGetText(attribute.text);

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
