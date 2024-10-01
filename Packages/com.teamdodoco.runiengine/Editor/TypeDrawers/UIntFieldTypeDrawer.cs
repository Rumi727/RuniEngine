#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(uint))]
    public sealed class UIntFieldTypeDrawer : NumberFieldTypeDrawer
    {
        UIntFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            if (uint.TryParse(EditorGUI.TextField(position, label, ((uint)(value ?? 0)).ToString()), out var result))
                return result;
            else
                return value;
        }
    }
}
