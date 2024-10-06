using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(int))]
    public sealed class IntFieldTypeDrawer : NumberFieldTypeDrawer
    {
        IntFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            int number;
            if (rangeAttribute != null)
                number = EditorGUI.IntSlider(position, label, (int)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else
                number = EditorGUI.IntField(position, label, (int)(value ?? 0));

            if (minAttribute != null)
                number = number.Max(minAttribute.min.FloorToInt());

            /*if (maxAttribute != null)
                number = number.Min(maxAttribute.max.FloorToInt());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
