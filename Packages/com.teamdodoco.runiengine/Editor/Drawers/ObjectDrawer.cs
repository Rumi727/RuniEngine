#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;
using EditorGUI = UnityEditor.EditorGUI;

namespace RuniEngine.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(object), true)]
    public sealed class ObjectAttributeDrawer : PropertyDrawer
    {
        AnimBool? animBool;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsChildrenIncluded(property))
            {
                animBool ??= new AnimBool(property.isExpanded);
                
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

                {
                    position.height = headHeight;

                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
                    animBool.target = property.isExpanded;
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
                        EditorGUI.PropertyField(position, property, IsChildrenIncluded(property));
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
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsChildrenIncluded(property))
            {
                animBool ??= new AnimBool(property.isExpanded);
                animBool.target = property.isExpanded;

                bool isExpanded = property.isExpanded;

                property.isExpanded = true;
                float orgHeight = EditorGUI.GetPropertyHeight(property, label);

                property.isExpanded = isExpanded;

                float childHeight = orgHeight - GetYSize(label, EditorStyles.foldout) - 3;
                return orgHeight - childHeight.Lerp(0f, animBool.faded);
            }
            else
                return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
