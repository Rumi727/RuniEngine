#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(sbyte))]
    public sealed class SbyteFieldTypeDrawer : NumberFieldTypeDrawer
    {
        SbyteFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            sbyte number;
            if (rangeAttribute != null)
                number = (sbyte)EditorGUI.IntSlider(position, label, (sbyte)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = (sbyte)EditorGUI.IntField(position, label, (sbyte)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((sbyte)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((sbyte)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
