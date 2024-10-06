using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(IList))]
    public sealed class ListTypeDrawer : ListTypeDrawerBase
    {
        ListTypeDrawer(SerializedTypeProperty property) : base(property) { }

        List<SerializedTypeListProperty?> listProperties = new List<SerializedTypeListProperty?>();

        int lastCount;
        public override SerializedTypeProperty? GetSerializedTypeProperty(IList list, int index)
        {
            if (lastCount != list.Count)
            {
                List<SerializedTypeListProperty?> listProperties = new List<SerializedTypeListProperty?>();
                for (int i = 0; i < list.Count; i++)
                {
                    SerializedTypeListProperty? listProperty = null;

                    if (property.propertyInfo != null)
                        listProperty = new SerializedTypeListProperty(GetListType(list), i, property.serializedType, property.propertyInfo, property);
                    else if (property.fieldInfo != null)
                        listProperty = new SerializedTypeListProperty(GetListType(list), i, property.serializedType, property.fieldInfo, property);

                    if (listProperty != null && i < this.listProperties.Count)
                        listProperty.isExpanded = this.listProperties[i]?.isExpanded ?? false;

                    listProperties.Add(listProperty);
                }

                this.listProperties = listProperties;
                lastCount = list.Count;
            }

            for (int i = 0; i < listProperties.Count; i++)
            {
                SerializedTypeListProperty? listProperty = listProperties[i];
                if (listProperty != null)
                {
                    listProperty.targetIndex = i;
                    listProperty.serializedType.targetObjects = property.serializedType.targetObjects;
                }
            }

            return listProperties[index];
        }

        public override IEnumerable<IList>? GetLists()
        {
            IEnumerable<object?> lists = property.GetNotNullValues();
            if (lists.Count() <= 0)
                return null;

            return lists.OfType<IList>();
        }

        public override void HeaderAddAction(IList list, int listIndex, int index)
        {
            object? defaultValue = GetListType(list).GetDefaultValue();
            if (list is Array array)
            {
                int? targetObjectIndex = property?.GetValues().IndexOf(list);
                if (targetObjectIndex != null && targetObjectIndex >= 0)
                    property?.SetValue(targetObjectIndex.Value, array.Insert(index, defaultValue));
            }
            else
                list.Insert(index, defaultValue);
        }

        public override void HeaderRemoveAction(IList list, int listIndex, int index)
        {
            if (list is Array array)
            {
                int? targetObjectIndex = property?.GetValues().IndexOf(list);
                if (targetObjectIndex != null && targetObjectIndex >= 0)
                    property?.SetValue(targetObjectIndex.Value, array.RemoveAt(index));
            }
            else
                list.RemoveAt(index);
        }

        public override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedTypeProperty? listProperty = GetSerializedTypeProperty(reorderableList.list, index);
            if (listProperty == null)
                return;

            listProperty.DrawGUI(rect, TryGetText("gui.element") + " " + index);
        }

        public override int OnAddCallback(ReorderableList list)
        {
            IEnumerable<IList>? lists = GetLists();
            if (lists == null)
                return -1;

            list.GrabKeyboardFocus();

            int index = int.MaxValue;
            foreach (var item in lists)
            {
                if (item.Count <= list.list.Count)
                {
                    object? defaultValue = GetListType(item).GetDefaultValue();

                    if (item is Array array)
                    {
                        int? targetObjectIndex = property?.GetValues().IndexOf(item);
                        if (targetObjectIndex != null && targetObjectIndex >= 0)
                            property?.SetValue(targetObjectIndex.Value, array.Add(defaultValue));
                    }
                    else
                        item.Add(defaultValue);

                    index = index.Min(item.Count);
                }
            }

            list.index = index;
            return index;
        }

        public override void OnRemoveCallback(ReorderableList list)
        {
            IEnumerable<IList>? lists = GetLists();
            if (lists == null)
                return;

            list.GrabKeyboardFocus();
            int[] selectedIndexes = list.selectedIndices.ToArray();

            int removeCount = 0;
            int minIndex = int.MaxValue;
            for (int i = 0; i < selectedIndexes.Length; i++)
            {
                int index = selectedIndexes[i] - removeCount;
                if (index >= list.count)
                    continue;

                foreach (var item in lists)
                {
                    if (item is Array array)
                    {
                        int? targetObjectIndex = property?.GetValues().IndexOf(item);
                        if (targetObjectIndex != null && targetObjectIndex >= 0)
                            property?.SetValue(targetObjectIndex.Value, array.RemoveAt(index));
                    }
                    else
                        item.RemoveAt(index);
                }

                minIndex = minIndex.Min(index);
                removeCount++;
            }

            list.index = (minIndex - 1).Clamp(0, list.count - 1);
        }

        public override void OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex) => listProperties.Move(oldIndex, newIndex);
    }
}
