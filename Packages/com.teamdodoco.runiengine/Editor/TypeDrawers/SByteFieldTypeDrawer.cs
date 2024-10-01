#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(sbyte))]
    public sealed class SByteFieldTypeDrawer : NumberFieldTypeDrawer
    {
        SByteFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => (sbyte)EditorGUI.IntField(position, label, (sbyte)(value ?? 0));
    }
}
