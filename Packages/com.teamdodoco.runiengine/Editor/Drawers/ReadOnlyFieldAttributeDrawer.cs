using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public sealed class ReadOnlyFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, property.IsChildrenIncluded());
            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label);
    }
}
