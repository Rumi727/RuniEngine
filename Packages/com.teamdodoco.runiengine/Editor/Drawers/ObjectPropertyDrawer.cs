#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;
using EditorGUI = UnityEditor.EditorGUI;
using EditorGUIUtility = UnityEditor.EditorGUIUtility;

namespace RuniEngine.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(object), true)]
    public sealed class ObjectPropertyDrawer : PropertyDrawer
    {
        readonly Dictionary<string, AnimBool> animBools = new Dictionary<string, AnimBool>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.IsChildrenIncluded() && !property.IsInArray())
            {
                AnimBool animBool;
                if (animBools.ContainsKey(property.propertyPath))
                    animBool = animBools[property.propertyPath];
                else
                {
                    animBool = new AnimBool(property.isExpanded);
                    animBools.Add(property.propertyPath, animBool);
                }

                float orgHeight;
                float headHeight = GetYSize(label, EditorStyles.foldout);
                int childCount;
                
                {
                    bool isExpanded = property.isExpanded;
                    property.isExpanded = true;
                    
                    orgHeight = EditorGUI.GetPropertyHeight(property, label);
                    childCount = property.Copy().CountInProperty() - 1;
                    
                    property.isExpanded = isExpanded;
                }

                position.y += 2;

                {
                    position.height = headHeight;

                    EditorGUI.BeginProperty(position, label, property);

                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
                    animBool.target = property.isExpanded;

                    EditorGUI.EndProperty();
                }

                if (animBool.faded != 0)
                {
                    float childHeight = orgHeight - headHeight;

                    position.x += 30;
                    position.width -= 30;

                    position.y += headHeight + 2;
                    if (animBool.faded != 1)
                        GUI.BeginClip(new Rect(0, 0, position.x + position.width, position.y + 0f.Lerp(childHeight, animBool.faded)));

                    property.Next(true);

                    int depth = property.depth;
                    for (int i = 0; i < childCount; i++)
                    {
                        if (property.depth != depth)
                        {
                            property.Next(false);
                            continue;
                        }

                        position.height = EditorGUI.GetPropertyHeight(property);

                        BeginLabelWidth(EditorGUIUtility.labelWidth - 15);
                        EditorGUI.PropertyField(position, property, property.IsChildrenIncluded());
                        EndLabelWidth();

                        position.y += position.height + 2;
                        property.Next(false);
                    }

                    if (animBool.faded != 1)
                        GUI.EndClip();
                }

                if (animBool.faded != 0 && animBool.faded != 1)
                    InspectorWindow.RepaintAllInspectors();
            }
            else
                EditorGUI.PropertyField(position, property, label, property.IsChildrenIncluded());
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.IsChildrenIncluded() && !property.IsInArray())
            {
                AnimBool animBool;
                if (animBools.ContainsKey(property.propertyPath))
                    animBool = animBools[property.propertyPath];
                else
                {
                    animBool = new AnimBool(property.isExpanded);
                    animBools.Add(property.propertyPath, animBool);
                }

                animBool.target = property.isExpanded;

                bool isExpanded = property.isExpanded;

                property.isExpanded = true;
                float orgHeight = EditorGUI.GetPropertyHeight(property, label, true);
                
                property.isExpanded = isExpanded;

                float childHeight = orgHeight - GetYSize(label, EditorStyles.foldout) - 3;
                return orgHeight - childHeight.Lerp(0f, animBool.faded);
            }
            else
                return EditorGUI.GetPropertyHeight(property, label, property.IsChildrenIncluded());
        }
    }
}
