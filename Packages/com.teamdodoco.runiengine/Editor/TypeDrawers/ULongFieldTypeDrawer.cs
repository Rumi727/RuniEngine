#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(ulong))]
    public sealed class ULongFieldTypeDrawer : NumberFieldTypeDrawer
    {
        ULongFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            EditorGUI.BeginChangeCheck();

            ulong number;
            if (rangeAttribute != null)
                number = (ulong)EditorGUI.IntSlider(position, label, (int)(value ?? 0), rangeAttribute.min.FloorToInt(), rangeAttribute.max.FloorToInt());
            else if (ulong.TryParse(EditorGUI.TextField(position, label, ((ulong)(value ?? 0)).ToString()), out var result))
                number = result;
            else
            {
                EditorGUI.EndChangeCheck();
                return value;
            }

            if (minAttribute != null)
                number = number.Max((ulong)minAttribute.min.Floor());

            /*if (maxAttribute != null)
                number = number.Min((ulong)maxAttribute.max.Floor());*/

            if (EditorGUI.EndChangeCheck())
                return number;

            return value;
        }
    }
}
