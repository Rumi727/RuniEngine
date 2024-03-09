#nullable enable
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(TooltipAttribute))]
    public sealed class TooltipAttributeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TooltipAttribute attribute = (TooltipAttribute)this.attribute;
            label.tooltip = EditorTool.TryGetText(attribute.text);

            EditorGUI.PropertyField(position, property, label, IsChildrenIncluded(property));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label);
    }
}
