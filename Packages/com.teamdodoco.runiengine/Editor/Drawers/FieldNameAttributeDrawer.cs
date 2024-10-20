using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FieldNameAttribute))]
    public sealed class FieldNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldNameAttribute attribute = (FieldNameAttribute)this.attribute;
            label.text = TryGetText(attribute.name);

            EditorGUI.PropertyField(position, property, label, property.IsChildrenIncluded());
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label);
    }
}
