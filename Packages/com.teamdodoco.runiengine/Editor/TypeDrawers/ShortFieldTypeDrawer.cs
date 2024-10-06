using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(short))]
    public sealed class ShortFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ShortFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            short number;
            if (rangeAttribute != null)
                number = (short)EditorGUI.IntSlider(position, label, (short)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = (short)EditorGUI.IntField(position, label, (short)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((short)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((short)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
