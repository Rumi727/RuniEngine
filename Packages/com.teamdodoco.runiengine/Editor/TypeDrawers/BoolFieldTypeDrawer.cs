#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(bool))]
    public sealed class BoolFieldTypeDrawer : FieldTypeDrawer
    {
        BoolFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override float GetPropertyHeight() => EditorTool.GetYSize(EditorStyles.toggle);

        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.Toggle(position, label, (bool)(value ?? false));
    }
}
