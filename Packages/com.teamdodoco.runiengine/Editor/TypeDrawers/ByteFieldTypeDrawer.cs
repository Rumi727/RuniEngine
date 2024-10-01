#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(byte))]
    public sealed class ByteFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ByteFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => (byte)EditorGUI.IntField(position, label, (byte)(value ?? 0));
    }
}
