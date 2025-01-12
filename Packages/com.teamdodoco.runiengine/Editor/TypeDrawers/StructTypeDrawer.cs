#nullable enable
using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Editor.TypeDrawers;
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    [CustomTypeDrawer(typeof(ValueType))]
    public class StructTypeDrawer : ObjectTypeDrawer
    {
        protected StructTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override void InternalOnGUI(Rect position, GUIContent? label)
        {
            if (property.DrawNullableButton(position, label, out bool isDrawed))
                return;

            if (!property.canRead)
            {
                DrawDefaultGUI(position, label);
                return;
            }

            if (isDrawed)
                position.width -= 42;

            float orgHeight = position.height;

            {
                position.height = EditorTool.GetYSize(EditorStyles.label);
                GUI.Label(position, label);
            }

            position.y += position.height + 3;
            position.height = orgHeight - (position.height + 3);

            SetChild();

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
            RectOffset rectOffset = EditorStyles.helpBox.border;

            position.xMin += rectOffset.left;
            position.xMax -= rectOffset.right;

            position.yMin += rectOffset.bottom;
            position.yMax -= rectOffset.top;

            InternalDrawStructGUI(position, label);
        }

        protected virtual void InternalDrawStructGUI(Rect position, GUIContent? label) => childSerializedType?.DrawGUI(position);

        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return EditorGUIUtility.singleLineHeight;

            RectOffset rectOffset = EditorStyles.helpBox.border;
            return rectOffset.top + rectOffset.bottom + EditorTool.GetYSize(EditorStyles.label) + 10 + InternalGetStructHeight();
        }

        protected virtual float InternalGetStructHeight() => childSerializedType?.GetPropertyHeight() ?? 0;
    }
}
