#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(double))]
    public sealed class DoubleFieldTypeDrawer : NumberFieldTypeDrawer
    {
        DoubleFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value) => EditorGUI.DoubleField(position, label, (double)(value ?? 0));
    }
}
