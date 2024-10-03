#nullable enable
using RuniEngine.Editor.SerializedTypes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using static RuniEngine.Editor.EditorTool;
using static UnityEditor.Progress;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(IList))]
    public sealed class ListTypeDrawer : ListTypeDrawerBase
    {
        internal ListTypeDrawer(SerializedTypeProperty property) : base(property) { }

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
                        listProperty = new SerializedTypeListProperty(i, property.serializedType, property.propertyInfo, property);
                    else if (property.fieldInfo != null)
                        listProperty = new SerializedTypeListProperty(i, property.serializedType, property.fieldInfo, property);

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

        public override void HeaderAddAction(IList list, int listIndex, int index) => list.Insert(index, GetListType(list).GetDefaultValue());

        public override void HeaderRemoveAction(IList list, int listIndex, int index) => list.RemoveAt(index);

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
                    index = index.Min(item.Add(GetListType(item).GetDefaultValue()));
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
                    item.RemoveAt(index);

                minIndex = minIndex.Min(index);
                removeCount++;
            }

            list.index = (minIndex - 1).Clamp(0, list.count - 1);
        }

        public override void OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex) => listProperties.Move(oldIndex, newIndex);
    }
}
