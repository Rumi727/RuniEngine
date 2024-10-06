using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using static RuniEngine.Editor.EditorTool;
using EditorGUI = UnityEditor.EditorGUI;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(object))]
    public class ObjectTypeDrawer : TypeDrawer
    {
        protected internal ObjectTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected SerializedType? childSerializedType { get; private set; }

        readonly AnimBool animBool = new AnimBool();
        readonly AnimFloat animFloat = new AnimFloat(0);

        protected void SetChild()
        {
            IEnumerable<object> enumerable = property.GetNotNullValues();
            if (enumerable.Any())
            {
                object?[] value = enumerable.ToArray();

                childSerializedType ??= new SerializedType(property.propertyType, property, false, value);
                childSerializedType.targetObjects = value;
                childSerializedType.metaData = property.serializedType.metaData;
            }
            else
                childSerializedType = null;
        }

        protected void SetChild(Type childType)
        {
            IEnumerable<object> enumerable = property.GetNotNullValues();
            if (enumerable.Any())
            {
                object?[] value = enumerable.ToArray();

                childSerializedType ??= new SerializedType(childType, property, false, value);
                childSerializedType.targetObjects = value;
                childSerializedType.metaData = property.serializedType.metaData;
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
                        DrawDefaultGUI(position, label);
                        return;
                    }

                    property.isExpanded = EditorGUI.Foldout(headPosition, property.isExpanded, label, true);
                }

                position.y += headHeight;
                position.height -= headHeight;

                position.x += 30;
                position.width -= 30;

                SetChild();

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
                DrawDefaultGUI(position, label);
        }

        protected override float InternalGetPropertyHeight()
        {
            if (property.propertyType.IsChildrenIncluded())
            {
                float orgHeight = GetYSize(EditorStyles.foldout);
                if (!property.canRead)
                    return orgHeight;

                SetChild();

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
                return base.InternalGetPropertyHeight();
        }
    }
}
