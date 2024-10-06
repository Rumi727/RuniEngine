using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(string))]
    public sealed class StringTypeDrawer : FieldTypeDrawer
    {
        StringTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return GetYSize(EditorStyles.label);

            IEnumerable<TextAreaAttribute> attributes = property.GetCustomAttributes<TextAreaAttribute>();
            if (attributes.Any())
            {
                TextAreaAttribute attribute = attributes.First();
                int line = attribute.maxLines;

                if (property.canRead)
                    line = (((string?)property.GetValue())?.Count(x => x == '\n') ?? 0) + 2;

                char[] content = new char[line.Clamp(attribute.minLines, attribute.maxLines)];
                Array.Fill(content, '\n');

                return GetYSize(new string(content), EditorStyles.textArea);
            }
            else
                return GetYSize(EditorStyles.textField);
        }

        public override object? DrawField(Rect position, GUIContent? label, object? value)
        {
            if (property.AttributeContains<TextAreaAttribute>())
            {
                float labelHeight = GetYSize(EditorStyles.label);
                {
                    Rect labelRect = position;

                    labelRect.x -= 1;
                    labelRect.height = labelHeight;

                    GUI.Label(labelRect, label);
                }

                position.y += labelHeight + 2;
                position.height -= labelHeight;

                return EditorGUI.TextArea(position, (string?)value);
            }
            else
                return EditorGUI.TextField(position, label, (string?)value);
        }
    }
}
