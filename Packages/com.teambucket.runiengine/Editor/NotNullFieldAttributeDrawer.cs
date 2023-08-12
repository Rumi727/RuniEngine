#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    [CustomPropertyDrawer(typeof(NotNullFieldAttribute), true)]
    public sealed class NotNullFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                position.width *= 0.6666666666666666666666666666666666666f;
                position.width -= 4;

                EditorGUI.PropertyField(position, property, label, true);

                position.x += position.width + 4;
                position.width *= 0.5f;
                position.width += 2;

                EditorGUI.HelpBox(position, "이 필드는 null 값일수 없습니다", MessageType.Error);
            }
            else
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
