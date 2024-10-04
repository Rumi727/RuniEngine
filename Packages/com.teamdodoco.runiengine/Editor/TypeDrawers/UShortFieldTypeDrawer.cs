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

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            ushort number;
            if (rangeAttribute != null)
                number = (ushort)EditorGUI.IntSlider(position, label, (ushort)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = (ushort)EditorGUI.IntField(position, label, (ushort)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((ushort)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((ushort)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
