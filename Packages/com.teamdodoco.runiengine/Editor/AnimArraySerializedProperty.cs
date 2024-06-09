#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;
using EditorGUI = UnityEditor.EditorGUI;

namespace RuniEngine.Editor
{
    public sealed class AnimArraySerializedProperty
    {
        public AnimArraySerializedProperty(SerializedProperty property)
        {
            this.property = property;

            float height;
            reorderableList = CreateReorderableList();
            if (property.isExpanded)
                height = reorderableList.GetHeight();
            else
                height = 0;

            animFloat = new AnimFloat(height);
        }

        public SerializedProperty property { get; }

        public AnimFloat? animFloat { get; } = null;
        public ReorderableList? reorderableList { get; } = null;

        public void Draw(Rect position, out float height) => Draw(position, null, out height);
        public void Draw(Rect position, GUIContent? label, out float height)
        {
            height = GetPropertyHeight(label);
            OnGUI(position, label);
        }

        public void DrawLayout(GUIContent? label = null)
        {
            Rect position = EditorGUILayout.GetControlRect(false, GetPropertyHeight(label));
            OnGUI(position, label);
        }

        void OnGUI(Rect position, GUIContent? label)
        {
            if (reorderableList == null)
                return;

            label ??= new GUIContent(property.displayName);

            bool isInArray = property.IsInArray();

            float headHeight = GetYSize(label, EditorStyles.foldoutHeader);
            position.height = headHeight;

            {
                Rect headerPosition = position;
                headerPosition.width -= 48;

                if (!isInArray)
                {
                    property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerPosition, property.isExpanded, label);
                    EditorGUI.EndFoldoutHeaderGroup();
                }
                else
                    property.isExpanded = EditorGUI.Foldout(headerPosition, property.isExpanded, label, true);
            }

            {
                Rect countPosition = position;
                countPosition.x += countPosition.width - 48;
                countPosition.width = 48;

                int count = EditorGUI.DelayedIntField(countPosition, property.arraySize);
                int addCount = count - property.arraySize;
                if (addCount > 0)
                {
                    for (int i = 0; i < addCount; i++)
                        property.InsertArrayElementAtIndex(property.arraySize);
                }
                else
                {
                    addCount = -addCount;
                    for (int i = 0; i < addCount; i++)
                        property.DeleteArrayElementAtIndex(property.arraySize - 1);
                }
            }

            position.y += headHeight + 2;

            if (!isInArray && animFloat != null)
            {
                if (property.isExpanded || animFloat.isAnimating)
                {
                    if (animFloat.isAnimating)
                        GUI.BeginClip(new Rect(0, 0, position.x + position.width, position.y + animFloat.value));

                    reorderableList.DoList(position);

                    if (animFloat.isAnimating)
                        GUI.EndClip();
                }

                if (animFloat.isAnimating)
                    InspectorWindow.RepaintAllInspectors();
            }
            else if (property.isExpanded)
                reorderableList.DoList(position);
        }

        public float GetPropertyHeight(GUIContent? label)
        {
            if (reorderableList == null)
                return 0;

            label ??= new GUIContent(property.displayName);

            float headerHeight = GetYSize(label, EditorStyles.foldoutHeader);
            float height;
            if (property.isExpanded)
                height = reorderableList.GetHeight() + 2;
            else
                height = 0;

            if (animFloat != null)
                animFloat.target = height;

            if (!property.IsInArray())
                return (animFloat?.value ?? 0) + headerHeight;
            else
                return height + headerHeight;
        }

        ReorderableList CreateReorderableList() => new ReorderableList(property.serializedObject, property)
        {
            multiSelect = true,
            headerHeight = 0,
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.x += 8;
                rect.width -= 8;
                rect.y += 1;
                rect.height -= 2;

                BeginMinLabelWidth(0, rect.width + 11, 0);
                
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, element.IsChildrenIncluded());

                EndLabelWidth();
            },
            onAddCallback = x =>
            {
                int index = property.arraySize;
                property.InsertArrayElementAtIndex(index);

                x.Select(index);
                x.GrabKeyboardFocus();
            },
            onRemoveCallback = x =>
            {
                if (x.selectedIndices.Count > 0)
                {
                    int removeCount = 0;
                    for (int i = 0; i < x.selectedIndices.Count; i++)
                    {
                        int index = x.selectedIndices[i] - removeCount;
                        if (index < 0 || index >= property.arraySize)
                            continue;

                        property.DeleteArrayElementAtIndex(index);
                        removeCount++;
                    }
                }
                else
                    property.DeleteArrayElementAtIndex(property.arraySize - 1);

                x.Select((x.index - 1).Clamp(0));
                x.GrabKeyboardFocus();
            },
            elementHeightCallback = i => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i))
        };
    }
}
