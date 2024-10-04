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

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            long number;
            if (rangeAttribute != null)
                number = EditorGUI.IntSlider(position, label, (int)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = EditorGUI.LongField(position, label, (long)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((long)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((long)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
