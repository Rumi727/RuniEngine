#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(ulong))]
    public sealed class ULongFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ULongFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            if (ulong.TryParse(EditorGUI.TextField(position, label, ((ulong)(value ?? 0)).ToString()), out var result))
                return result;
            else
                return value;
        }
    }
}
