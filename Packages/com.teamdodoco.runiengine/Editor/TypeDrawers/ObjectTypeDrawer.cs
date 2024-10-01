#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEngine;

using EditorGUI = UnityEditor.EditorGUI;
using static RuniEngine.Editor.EditorTool;
using System.Linq;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(object))]
    public sealed class ObjectTypeDrawer : TypeDrawer
    {
        internal ObjectTypeDrawer(SerializedTypeProperty property) : base(property) { }

        SerializedType? childSerializedType;
        AnimBool animBool = new AnimBool();

        bool SetChildProperty()
        {
            object?[] value = property.GetNotNullValues().ToArray();
            if (value.Length > 0)
            {
                childSerializedType ??= new SerializedType(property.propertyType, property, false, value);
                childSerializedType.targetObjects = value;
            }
            else
                childSerializedType = null;

            return value == null;
        }

        public override void OnGUI(Rect position, GUIContent? label)
        {
            if (property.propertyType.IsChildrenIncluded())
            {
                float headHeight = GetYSize(EditorStyles.foldout);
                {
                    Rect headPosition = position;
                    headPosition.height = headHeight;

                    if (property.DrawNullableButton(headPosition, label, out _))
                        return;

                    if (!property.canRead)
                    {
                        base.OnGUI(position, label);
                        return;
                    }

                    property.isExpanded = EditorGUI.Foldout(headPosition, property.isExpanded, label, true);
                }

                position.y += headHeight;
                position.height -= headHeight;

                position.x += 30;
                position.width -= 30;

                SetChildProperty();

                if (!property.isInArray)
                {
                    animBool.target = property.isExpanded;

                    if (animBool.isAnimating)
                    {
                        float childHeight = childSerializedType?.GetPropertyHeight() ?? 0;

                        GUI.BeginClip(new Rect(0, 0, position.x + position.width, position.y + 0f.Lerp(childHeight, animBool.faded)));
                        childSerializedType?.DrawGUI(position);
                        GUI.EndClip();

                        RepaintCurrentWindow();
                        return;
                    }
                    else if (property.isExpanded)
                        childSerializedType?.DrawGUI(position);
                }
                else if (property.isExpanded)
                    childSerializedType?.DrawGUI(position);
            }
            else
                base.OnGUI(position, label);
        }

        public override float GetPropertyHeight()
        {
            if (property.propertyType.IsChildrenIncluded())
            {
                float orgHeight = GetYSize(EditorStyles.foldout);
                if (!property.canRead || SetChildProperty())
                    return orgHeight;

                float childHeight = childSerializedType?.GetPropertyHeight() ?? 0;
                /*if (property.isExpanded)
                    return orgHeight + childHeight;
                else
                    return orgHeight;*/
                
                if (!property.isInArray)
                {
                    animBool.target = property.isExpanded;
                    return orgHeight + 0f.Lerp(childHeight, animBool.faded);
                }
                else
                {
                    if (property.isExpanded)
                        return orgHeight + childHeight;
                    else
                        return orgHeight;
                }
            }
            else
                return base.GetPropertyHeight();
        }
    }
}
