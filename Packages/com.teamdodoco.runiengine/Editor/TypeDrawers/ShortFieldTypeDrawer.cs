#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(short))]
    public sealed class ShortFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ShortFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => (short)EditorGUI.IntField(position, label, (short)(value ?? 0));
    }
}
