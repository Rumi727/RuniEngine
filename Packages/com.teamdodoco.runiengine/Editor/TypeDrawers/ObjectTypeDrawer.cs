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
        readonly AnimBool animBool = new AnimBool();
        readonly AnimFloat animFloat = new AnimFloat(0);

        void SetChildProperty()
        {
            object?[] value = property.GetNotNullValues().ToArray();
            if (value.Length > 0)
            {
                childSerializedType ??= new SerializedType(property.propertyType, property, false, value);
                childSerializedType.targetObjects = value;
            }
            else
                childSerializedType = null;
        }

        protected override void InternalOnGUI(Rect position, GUIContent? label)
        {
            if (property.propertyType.IsChildrenIncluded())
            {
                float headHeight = GetYSize(EditorStyles.foldout);
                {
                    Rect headPosition = position;
                    headPosition.height = headHeight;

                    if (property.DrawNullableButton(headPosition, label, out _))
                    {
                        animBool.value = false;
                        animFloat.target = 0;

                        if (animFloat.isAnimating)
                            RepaintCurrentWindow();

                        return;
                    }

                    if (!property.canRead)
                    {
                        base.InternalOnGUI(position, label);
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
                        animFloat.value = childHeight;

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
                base.InternalOnGUI(position, label);
        }

        public override float GetPropertyHeight()
        {
            if (property.propertyType.IsChildrenIncluded())
            {
                float orgHeight = GetYSize(EditorStyles.foldout);
                if (!property.canRead)
                    return orgHeight;

                SetChildProperty();

                if (childSerializedType == null)
                    return orgHeight + animFloat.value;
                
                if (!property.isInArray && animBool.isAnimating)
                {
                    float childHeight = childSerializedType.GetPropertyHeight();

                    animBool.target = property.isExpanded;
                    return orgHeight + 0f.Lerp(childHeight, animBool.faded);
                }
                else
                {
                    if (property.isExpanded)
                        return orgHeight + childSerializedType.GetPropertyHeight();
                    else
                        return orgHeight;
                }
            }
            else
                return base.GetPropertyHeight();
        }
    }
}
