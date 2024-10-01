#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(long))]
    public sealed class LongFieldTypeDrawer : NumberFieldTypeDrawer
    {
        LongFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.LongField(position, label, (long)(value ?? 0));
    }
}
