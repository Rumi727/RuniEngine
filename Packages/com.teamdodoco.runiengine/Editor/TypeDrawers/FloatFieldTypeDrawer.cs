using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(float))]
    public sealed class FloatFieldTypeDrawer : NumberFieldTypeDrawer
    {
        FloatFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            float number;
            if (rangeAttribute != null)
                number = (float)EditorGUI.Slider(position, label, (float)(value ?? 0), rangeAttribute.min, rangeAttribute.max);
            else
                number = (float)EditorGUI.FloatField(position, label, (float)(value ?? 0));

            if (minAttribute != null)
                number = number.Max((float)minAttribute.min);

            /*if (maxAttribute != null)
                number = number.Min((float)maxAttribute.max);*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
