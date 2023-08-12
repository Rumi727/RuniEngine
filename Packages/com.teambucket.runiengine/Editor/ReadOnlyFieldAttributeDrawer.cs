#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute), true)]
    public sealed class ReadOnlyFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool preGUIEnabled = GUI.enabled;

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = preGUIEnabled;
        }
    }
}
