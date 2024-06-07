#nullable enable
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ISerializableDictionary), true)]
    public sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        ReorderableList? reorderableList;
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
            
            reorderableList ??= CreateReorderableList(serializedObject, property.displayName, key, value);
            reorderableList.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GetListProperty(property, out SerializedObject serializedObject, out SerializedProperty? key, out SerializedProperty? value);
            if (key == null || value == null)
                return base.GetPropertyHeight(property, label);

            reorderableList ??= CreateReorderableList(serializedObject, property.displayName, key, value);
            return reorderableList.GetHeight();
        }

        public static void GetListProperty(SerializedProperty property, out SerializedObject serializedObject, out SerializedProperty? key, out SerializedProperty? value)
        {
            property = property.Copy();

            serializedObject = property.serializedObject;
            key = null;
            value = null;

            while (property.Next(true))
            {
                if (key != null && value != null)
                    break;

                if (property.name == nameof(ISerializableDictionary.serializableKeys))
                    key = property.Copy();
                else if (property.name == nameof(ISerializableDictionary.serializableValues))
                    value = property.Copy();
            }
        }

        public static ReorderableList CreateReorderableList(SerializedObject serializedObject, string label, SerializedProperty? key, SerializedProperty? value) => new ReorderableList(serializedObject, key)
        {
            multiSelect = true,
            drawHeaderCallback = x => GUI.Label(x, label),
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

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, keyElement, new GUIContent(keyLabel), key.IsChildrenIncluded());

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
                EditorGUI.PropertyField(rect, value.GetArrayElementAtIndex(index), new GUIContent(valueLabel), value.IsChildrenIncluded());
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
            },
            onRemoveCallback = x =>
            {
                if (key == null || value == null)
                    return;

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
            },
            onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
            {
                if (key == null || value == null)
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
