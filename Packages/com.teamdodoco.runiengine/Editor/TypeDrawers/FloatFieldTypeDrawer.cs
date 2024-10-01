#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(float))]
    public sealed class FloatFieldTypeDrawer : NumberFieldTypeDrawer
    {
        FloatFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.FloatField(position, label, (float)(value ?? 0));
    }
}
