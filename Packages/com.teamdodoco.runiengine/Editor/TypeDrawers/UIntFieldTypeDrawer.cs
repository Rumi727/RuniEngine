using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(uint))]
    public sealed class UIntFieldTypeDrawer : NumberFieldTypeDrawer
    {
        UIntFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            uint number;
            if (rangeAttribute != null)
                number = (uint)EditorGUI.IntSlider(position, label, (int)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else if (uint.TryParse(EditorGUI.TextField(position, label, ((uint)(value ?? 0)).ToString()), out var result))
                number = result;
            else
            {
                EditorGUI.EndChangeCheck();
                return value;
            }

            if (minAttribute != null)
                number = number.Max((uint)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((uint)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
