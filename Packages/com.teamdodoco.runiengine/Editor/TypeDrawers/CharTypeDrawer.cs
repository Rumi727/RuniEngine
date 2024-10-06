using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(char))]
    public sealed class CharTypeDrawer : FieldTypeDrawer
    {
        CharTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override float InternalGetPropertyHeight() => EditorTool.GetYSize(EditorStyles.textField);
        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.TextField(position, label, ((char)(value ?? 0)).ToString())[0];
    }
}
