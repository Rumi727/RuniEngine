#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Attribute
{
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute), true)]
    public sealed class ReadOnlyFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
