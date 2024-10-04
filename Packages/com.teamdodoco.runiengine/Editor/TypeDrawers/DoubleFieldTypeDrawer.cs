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

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            double number;
            if (rangeAttribute != null)
                number = (double)EditorGUI.Slider(position, label, (float)(value ?? 0), rangeAttribute.min, rangeAttribute.max);
            else
                number = EditorGUI.DoubleField(position, label, (double)(value ?? 0));

            if (minAttribute != null)
                number = number.Max(minAttribute.min);

            /*if (maxAttribute != null)
                number = number.Min(maxAttribute.max);*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
