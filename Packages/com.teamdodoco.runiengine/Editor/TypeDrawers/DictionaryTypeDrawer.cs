#nullable enable
using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(IDictionary))]
    public sealed class DictionaryTypeDrawer : ListTypeDrawerBase
    {
        DictionaryTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override bool canEditMultipleObjects => false;



        List<SerializedTypeDictionaryKeyProperty?> listKeyProperties = new List<SerializedTypeDictionaryKeyProperty?>();
        List<SerializedTypeDictionaryValueProperty?> listValueProperties = new List<SerializedTypeDictionaryValueProperty?>();

        int lastKeyCount;
        public SerializedTypeDictionaryKeyProperty? GetSerializedTypeKeyProperty(IList list, int index)
        {
            for (int i = 0; i < listKeyProperties.Count.Min(list.Count); i++)
            {
                SerializedTypeDictionaryKeyProperty? listProperty = listKeyProperties[i];
                if (listProperty == null)
                    continue;

                listProperty.serializedType.targetObjects = property.serializedType.targetObjects;

                if (listProperty.GetValue() != ((DictionaryEntry)list[i]).Key)
                    lastKeyCount = -1;
            }

            if (lastKeyCount == list.Count)
                return listKeyProperties[index];

            List<SerializedTypeDictionaryKeyProperty?> listProperties = new List<SerializedTypeDictionaryKeyProperty?>();
            for (int i = 0; i < list.Count; i++)
            {
                SerializedTypeDictionaryKeyProperty? listProperty = null;
                DictionaryEntry entry = (DictionaryEntry)list[i];

                Type keyType = GetDictionaryType(GetDictionary()?.GetType() ?? typeof(object)).key;
                if (property.propertyInfo != null)
                    listProperty = new SerializedTypeDictionaryKeyProperty(keyType, entry.Key, property.serializedType, property.propertyInfo, property);
                else if (property.fieldInfo != null)
                    listProperty = new SerializedTypeDictionaryKeyProperty(keyType, entry.Key, property.serializedType, property.fieldInfo, property);

                if (listProperty != null && i < listKeyProperties.Count)
                    listProperty.isExpanded = listKeyProperties[i]?.isExpanded ?? false;

                listProperties.Add(listProperty);
            }

            listKeyProperties = listProperties;
            lastKeyCount = list.Count;

            return listKeyProperties[index];
        }

        int lastValueCount;
        public override SerializedTypeProperty? GetSerializedTypeProperty(IList list, int index)
        {
            for (int i = 0; i < listValueProperties.Count.Min(list.Count); i++)
            {
                SerializedTypeDictionaryValueProperty? listProperty = listValueProperties[i];
                if (listProperty == null)
                    continue;

                listProperty.serializedType.targetObjects = property.serializedType.targetObjects;

                if (listProperty.GetValue() != ((DictionaryEntry)list[i]).Value)
                    lastValueCount = -1;
            }

            if (lastValueCount != list.Count)
            {
                List<SerializedTypeDictionaryValueProperty?> listProperties = new List<SerializedTypeDictionaryValueProperty?>();

                for (int i = 0; i < list.Count; i++)
                {
                    SerializedTypeDictionaryValueProperty? listProperty = null;
                    DictionaryEntry entry = (DictionaryEntry)list[i];

                    Type valueType = GetDictionaryType(GetDictionary()?.GetType() ?? typeof(object)).value;
                    if (property.propertyInfo != null)
                        listProperty = new SerializedTypeDictionaryValueProperty(valueType, entry.Key, property.serializedType, property.propertyInfo, property);
                    else if (property.fieldInfo != null)
                        listProperty = new SerializedTypeDictionaryValueProperty(valueType, entry.Key, property.serializedType, property.fieldInfo, property);

                    if (listProperty != null && i < listValueProperties.Count)
                        listProperty.isExpanded = listValueProperties[i]?.isExpanded ?? false;

                    listProperties.Add(listProperty);
                }

                listValueProperties = listProperties;
                lastValueCount = list.Count;
            }

            return listValueProperties[index];
        }

        public IEnumerable<IDictionary>? GetDictionarys()
        {
            IEnumerable<object?> lists = property.GetNotNullValues();
            if (lists.Count() <= 0)
                return null;

            return lists.OfType<IDictionary>();
        }

        public IDictionary? GetDictionary() => GetDictionarys().FirstOrDefault();

        public override IEnumerable<IList>? GetLists() => GetDictionarys()?.Select(x => x.ToGeneric().ToList());

        public override void HeaderAddAction(IList list, int listIndex, int index)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return;

            int dictionaryIndex = 0;
            foreach (var item in lists)
            {
                if (listIndex == dictionaryIndex)
                {
                    (Type keyType, Type valueType) = GetDictionaryType(item);

                    object key = keyType.GetDefaultValueNotNull();
                    if (!item.Contains(key))
                        item.Add(key, valueType.GetDefaultValue());

                    return;
                }

                dictionaryIndex++;
            }

            list.Insert(0, GetListType(list).GetDefaultValue());
        }

        public override void HeaderRemoveAction(IList list, int listIndex, int index)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return;

            int dictionaryIndex = 0;
            foreach (var item in lists)
            {
                if (listIndex == dictionaryIndex)
                {
                    object? keyObject = null;
                    int keyIndex = 0;
                    foreach (var item2 in item.Keys)
                    {
                        if (keyIndex == index)
                        {
                            keyObject = item2;
                            break;
                        }

                        keyIndex++;
                    }

                    item.Remove(keyObject);
                    return;
                }

                dictionaryIndex++;
            }

            list.RemoveAt(index);
        }

        public override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            string keyLabel = TryGetText("gui.key");
            string valueLabel = TryGetText("gui.value");

            rect.width /= 2;
            rect.width -= 10;

            //키
            {
                SerializedTypeDictionaryKeyProperty? keyProperty = GetSerializedTypeKeyProperty(reorderableList.list, index);
                if (keyProperty == null)
                    return;

                BeginLabelWidth(keyLabel);

                rect.height = keyProperty.GetPropertyHeight();

                EditorGUI.BeginChangeCheck();
                keyProperty.DrawGUI(rect, keyLabel);
                if (EditorGUI.EndChangeCheck())
                    lastValueCount = -1;

                EndLabelWidth();
            }

            rect.x += rect.width + 20;

            //값
            {
                SerializedTypeDictionaryValueProperty? valueProperty = (SerializedTypeDictionaryValueProperty?)GetSerializedTypeProperty(reorderableList.list, index);
                if (valueProperty == null)
                    return;

                BeginLabelWidth(valueLabel);

                rect.height = valueProperty.GetPropertyHeight();
                valueProperty.DrawGUI(rect, valueLabel);

                EndLabelWidth();
            }
        }

        public override float ElementHeightCallback(IList list, int index)
        {
            float height = 0;

            //키
            {
                SerializedTypeDictionaryKeyProperty? keyProperty = GetSerializedTypeKeyProperty(reorderableList.list, index);
                if (keyProperty != null)
                    height = height.Max(keyProperty.GetPropertyHeight());
            }

            //값
            {
                SerializedTypeDictionaryValueProperty? valueProperty = (SerializedTypeDictionaryValueProperty?)GetSerializedTypeProperty(reorderableList.list, index);
                if (valueProperty != null)
                    height = height.Max(valueProperty.GetPropertyHeight());
            }

            return height;
        }

        public override int OnAddCallback(ReorderableList list)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return -1;

            list.GrabKeyboardFocus();

            int index = int.MaxValue;
            foreach (var item in lists)
            {
                if (item.Count <= list.list.Count)
                {
                    (Type key, Type value) = GetDictionaryType(item);
                    item.Add(key.GetDefaultValueNotNull(), value.GetDefaultValue());

                    index = index.Min(item.Count - 1);
                }
            }

            list.index = index;
            return index;
        }

        public override void OnRemoveCallback(ReorderableList list)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return;

            list.GrabKeyboardFocus();
            ReadOnlyCollection<int> selectedIndexes = list.selectedIndices;

            int minIndex = 0;
            for (int i = 0; i < selectedIndexes.Count; i++)
            {
                int index = selectedIndexes[i];
                if (index >= list.count)
                    continue;

                foreach (var item in lists)
                {
                    int keyIndex = 0;
                    object? removeKey = null;

                    foreach (var key in item.Keys)
                    {
                        if (keyIndex == list.index)
                        {
                            removeKey = key;
                            break;
                        }

                        keyIndex++;
                    }

                    item.Remove(removeKey);
                }

                minIndex = minIndex.Min(index);
            }

            list.index = minIndex.Clamp(0, list.count - 1);
        }

        public override void OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return;

            foreach (var item in lists)
            {
                List<DictionaryEntry> genericList = item.ToGeneric().ToList();
                genericList.Move(oldIndex, newIndex);

                item.Clear();
                for (int i = 0; i < genericList.Count; i++)
                {
                    DictionaryEntry entry = genericList[i];
                    item.Add(entry.Key, entry.Value);
                }
            }

            listKeyProperties.Move(oldIndex, newIndex);
            listValueProperties.Move(oldIndex, newIndex);
        }

        public override bool CanAddCallback(ReorderableList list)
        {
            IEnumerable<IDictionary>? lists = GetDictionarys();
            if (lists == null)
                return false;

            foreach (var item in lists)
            {
                Type keyType = GetDictionaryType(item).key;
                if (item.Count <= list.list.Count && item.Contains(keyType.GetDefaultValueNotNull()))
                    return false;
            }

            return true;
        }
    }
}
