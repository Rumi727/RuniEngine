#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(ushort))]
    public sealed class UShortFieldTypeDrawer : NumberFieldTypeDrawer
    {
        UShortFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => (ushort)EditorGUI.IntField(position, label, (ushort)(value ?? 0));
    }
}
