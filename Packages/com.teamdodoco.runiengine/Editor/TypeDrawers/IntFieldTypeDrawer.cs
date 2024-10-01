#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(int))]
    public sealed class IntFieldTypeDrawer : NumberFieldTypeDrawer
    {
        IntFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.IntField(position, label, (int)(value ?? 0));
    }
}
