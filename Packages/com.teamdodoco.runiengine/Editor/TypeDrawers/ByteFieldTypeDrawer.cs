using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(byte))]
    public sealed class ByteFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ByteFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            byte number;
            if (rangeAttribute != null)
                number = (byte)EditorGUI.IntSlider(position, label, (byte)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = (byte)EditorGUI.IntField(position, label, (byte)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((byte)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((byte)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
