#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;
using EditorGUI = UnityEditor.EditorGUI;
using EditorGUIUtility = UnityEditor.EditorGUIUtility;

namespace RuniEngine.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ISerializableDictionary), true)]
    public sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        readonly Dictionary<string, AnimFloat> animFloats = new Dictionary<string, AnimFloat>();
        readonly Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetListProperty(property, out SerializedObject serializedObject, out SerializedProperty? key, out SerializedProperty? value);
            if (key == null || value == null)
            {
#pragma warning disable UNT0027 // Do not call PropertyDrawer.OnGUI()
                base.OnGUI(position, property, label);
#pragma warning restore UNT0027 // Do not call PropertyDrawer.OnGUI()
                return;
            }

            bool isInArray = property.IsInArray();

            float headHeight = GetYSize(label, EditorStyles.foldoutHeader);
            position.height = headHeight;

            {
                Rect headerPosition = position;
                headerPosition.width -= 48;

                EditorGUI.BeginProperty(headerPosition, label, property);

                if (!isInArray)
                {
                    property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerPosition, property.isExpanded, label);
                    EditorGUI.EndFoldoutHeaderGroup();
                }
                else
                    property.isExpanded = EditorGUI.Foldout(headerPosition, property.isExpanded, label, true);

                EditorGUI.EndProperty();
            }

            {
                Rect countPosition = position;
                countPosition.x += countPosition.width - 48;
                countPosition.width = 48;

                int count = EditorGUI.DelayedIntField(countPosition, key.arraySize);
                int addCount = count - key.arraySize;
                if (addCount > 0)
                {
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = key.arraySize;

                        key.InsertArrayElementAtIndex(index);
                        value.InsertArrayElementAtIndex(index);

                        //InsertArrayElementAtIndex 함수는 값을 복제하기 때문에 키를 기본값으로 정해줘야 제대로 생성할 수 있게 됨
                        key.GetArrayElementAtIndex(index).SetDefaultValue();
                    }
                }
                else
                {
                    addCount = -addCount;
                    for (int i = 0; i < addCount; i++)
                    {
                        int index = key.arraySize - 1;

                        key.DeleteArrayElementAtIndex(index);
                        value.DeleteArrayElementAtIndex(index);
                    }
                }
            }

            position.y += headHeight + 2;

            if (!isInArray)
            {
                AnimFloat? animFloat = GetAnimFloat(property);
                if (animFloat == null)
                    return;

                if (property.isExpanded || animFloat.isAnimating)
                {
                    if (animFloat.isAnimating)
                        GUI.BeginClip(new Rect(0, 0, position.x + position.width, position.y + animFloat.value));

                    ReorderableList reorderableList = GetReorderableList(serializedObject, property, key, value);
                    reorderableList.DoList(position);

                    if (animFloat.isAnimating)
                        GUI.EndClip();
                }

                if (animFloat.isAnimating)
                    InspectorWindow.RepaintAllInspectors();
            }
            else if (property.isExpanded)
            {
                ReorderableList reorderableList = GetReorderableList(serializedObject, property, key, value);
                reorderableList.DoList(position);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GetListProperty(property, out SerializedObject serializedObject, out SerializedProperty? key, out SerializedProperty? value);
            if (key == null || value == null)
                return base.GetPropertyHeight(property, label);

            float headerHeight = GetYSize(label, EditorStyles.foldoutHeader);
            float height;
            ReorderableList reorderableList = GetReorderableList(serializedObject, property, key, value);
            if (property.isExpanded)
                height = reorderableList.GetHeight() + 2;
            else
                height = 0;

            if (!property.IsInArray())
            {
                AnimFloat animFloat = CreateAnimFloat(property, height);
                animFloat.target = height;

                return animFloat.value + headerHeight;
            }
            else
                return height + headerHeight;
        }

        public AnimFloat CreateAnimFloat(SerializedProperty property, float height)
        {
            if (animFloats.ContainsKey(property.propertyPath))
                return animFloats[property.propertyPath];
            else
                return animFloats[property.propertyPath] = new AnimFloat(height);
        }

        public AnimFloat? GetAnimFloat(SerializedProperty property)
        {
            if (animFloats.ContainsKey(property.propertyPath))
                return animFloats[property.propertyPath];

            return null;
        }

        public static void GetListProperty(SerializedProperty property, out SerializedObject serializedObject, out SerializedProperty? key, out SerializedProperty? value)
        {
            serializedObject = property.serializedObject;
            
            key = property.FindPropertyRelative(nameof(ISerializableDictionary.serializableKeys));
            value = property.FindPropertyRelative(nameof(ISerializableDictionary.serializableValues));
        }

        public ReorderableList GetReorderableList(SerializedObject serializedObject, SerializedProperty property, SerializedProperty? key, SerializedProperty? value)
        {
            if (reorderableLists.TryGetValue(property.propertyPath, out ReorderableList result))
                return result;
            else
            {
                return reorderableLists[property.propertyPath] = new ReorderableList(serializedObject, key)
                {
                    multiSelect = true,
                    headerHeight = 0,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        if (key == null || value == null)
                            return;

                        string keyLabel = TryGetText("gui.key");
                        string valueLabel = TryGetText("gui.value");

                        rect.width /= 2;
                        rect.width -= 10;

                        BeginLabelWidth(keyLabel);

                        SerializedProperty keyElement = key.GetArrayElementAtIndex(index);
                        object? lastValue = keyElement.boxedValue;

                        rect.height = EditorGUI.GetPropertyHeight(keyElement);

                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(rect, keyElement, new GUIContent(keyLabel), keyElement.IsChildrenIncluded());

                        //중복 감지
                        if (EditorGUI.EndChangeCheck())
                        {
                            for (int i = 0; i < key.arraySize; i++)
                            {
                                if (index != i && Equals(key.GetArrayElementAtIndex(i).boxedValue, keyElement.boxedValue))
                                {
                                    keyElement.boxedValue = lastValue;
                                    break;
                                }
                            }
                        }

                        EndLabelWidth();

                        rect.x += rect.width + 20;

                        BeginLabelWidth(valueLabel);

                        SerializedProperty valueElement = value.GetArrayElementAtIndex(index);
                        rect.height = EditorGUI.GetPropertyHeight(valueElement);

                        EditorGUI.PropertyField(rect, valueElement, new GUIContent(valueLabel), valueElement.IsChildrenIncluded());

                        EndLabelWidth();
                    },
                    onAddCallback = x =>
                    {
                        if (key == null || value == null)
                            return;

                        int index = key.arraySize;

                        key.InsertArrayElementAtIndex(index);
                        value.InsertArrayElementAtIndex(index);

                        //InsertArrayElementAtIndex 함수는 값을 복제하기 때문에 키를 기본값으로 정해줘야 제대로 생성할 수 있게 됨
                        key.GetArrayElementAtIndex(index).SetDefaultValue();

                        x.Select(index);
                        x.GrabKeyboardFocus();
                    },
                    onRemoveCallback = x =>
                    {
                        if (key == null || value == null)
                            return;

                        if (x.selectedIndices.Count > 0)
                        {
                            int removeCount = 0;
                            for (int i = 0; i < x.selectedIndices.Count; i++)
                            {
                                int index = x.selectedIndices[i] - removeCount;
                                if (index < 0 || index >= key.arraySize)
                                    continue;

                                key.DeleteArrayElementAtIndex(index);
                                value.DeleteArrayElementAtIndex(index);

                                removeCount++;
                            }
                        }
                        else
                        {
                            key.DeleteArrayElementAtIndex(key.arraySize - 1);
                            value.DeleteArrayElementAtIndex(value.arraySize - 1);
                        }

                        x.Select((x.index - 1).Clamp(0));
                        x.GrabKeyboardFocus();
                    },
                    onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
                    {
                        if (value == null)
                            return;

                        value.MoveArrayElement(oldIndex, newIndex);
                    },
                    onCanAddCallback = x =>
                    {
                        if (key == null)
                            return false;

                        for (int i = 0; i < key.arraySize; i++)
                        {
                            SerializedProperty keyElement = key.GetArrayElementAtIndex(i);
                            if (keyElement.propertyType == SerializedPropertyType.String)
                            {
                                if (string.IsNullOrEmpty(keyElement.stringValue))
                                    return false;
                            }
                            else
                            {
                                object? boxedValue = keyElement.boxedValue;

                                if (boxedValue == null)
                                    return false;
                                if (boxedValue == boxedValue.GetType().GetDefaultValue())
                                    return false;
                            }
                        }

                        return true;
                    },
                    elementHeightCallback = i =>
                    {
                        if (key == null || value == null)
                            return EditorGUIUtility.singleLineHeight;

                        float height = EditorGUI.GetPropertyHeight(key.GetArrayElementAtIndex(i));
                        height = height.Max(EditorGUI.GetPropertyHeight(value.GetArrayElementAtIndex(i)));

                        return height;
                    }
                };
            }
        }
    }
}
