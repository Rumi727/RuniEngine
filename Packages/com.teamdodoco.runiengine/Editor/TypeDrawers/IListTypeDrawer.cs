#nullable enable
using RuniEngine.Editor.SerializedTypes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    [CustomTypeDrawer(typeof(IList))]
    public sealed class IListTypeDrawer : TypeDrawer
    {
        internal IListTypeDrawer(SerializedTypeProperty property) : base(property) { }

        AnimFloat animFloat = new AnimFloat(0);

        ReorderableList reorderableList = new ReorderableList(null, typeof(object));
        List<SerializedTypeListProperty?> listProperties = new List<SerializedTypeListProperty?>();

        IList? lastList;
        int lastCount;

        public override void OnGUI(Rect position, GUIContent? label)
        {
            float headHeight = GetYSize(EditorStyles.foldoutHeader);
            Rect headPosition = position;
            headPosition.height = headHeight;

            if (property.DrawNullableButton(headPosition, label, out bool isDrawed))
                return;

            if (isDrawed)
                headPosition.width -= 42;

            if (!property.canRead)
            {
                base.OnGUI(headPosition, label);
                return;
            }

            IList list = property.GetNotNullValues().OfType<IList>().MinBy(x => x.Count);
            if (GetListType(list) == typeof(object))
            {
                base.OnGUI(headPosition, label);
                return;
            }

            property.isExpanded = ListHeader(headPosition, list, label ?? GUIContent.none, property.isExpanded);

            position.y += headHeight;

            if (!property.isInArray)
            {
                float height;

                reorderableList = GetReorderableList(list);
                if (property.isExpanded)
                    height = reorderableList.GetHeight() + 2;
                else
                    height = 0;

                animFloat.target = height;

                if (animFloat.isAnimating)
                {
                    GUI.BeginClip(new Rect(0, 0, position.x + position.width, position.y + animFloat.value));
                    reorderableList.DoList(position);
                    GUI.EndClip();
                }
                else if (property.isExpanded)
                    reorderableList.DoList(position);

                if (animFloat.isAnimating)
                    RepaintCurrentWindow();
            }
            else if (property.isExpanded)
            {
                reorderableList = GetReorderableList(list);
                reorderableList.DoList(position);
            }
        }

        public override float GetPropertyHeight()
        {
            if (!property.canRead)
                return base.GetPropertyHeight();

            IEnumerable<object?> lists = property.GetNotNullValues();
            if (lists.Count() <= 0)
                return base.GetPropertyHeight();

            IList list = lists.OfType<IList>().MinBy(x => x.Count);

            float headerHeight = GetYSize(EditorStyles.foldoutHeader);
            float height;

            reorderableList = GetReorderableList(list);
            if (property.isExpanded)
                height = reorderableList.GetHeight() + 2;
            else
                height = 0;

            if (!property.isInArray)
            {
                animFloat.target = height;
                return headerHeight + animFloat.value;
            }
            else
                return height + headerHeight;
        }

        public ReorderableList GetReorderableList(IList list)
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
                    listProperty.serializedType.targetObjects = property.serializedType.targetObjects;
            }

            if (lastList == list)
                return reorderableList;
            else
            {
                lastList = list;
                return new ReorderableList(list, GetListType(list))
                {
                    multiSelect = true,
                    headerHeight = 0,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        SerializedTypeListProperty? listProperty = listProperties[index];
                        if (listProperty == null)
                            return;

                        rect.x += 8;
                        rect.width -= 8;
                        rect.y += 1;
                        rect.height -= 2;

                        BeginMinLabelWidth(0, rect.width + 11, 0);
                        listProperty.DrawGUI(rect, "Element " + index);
                        EndLabelWidth();
                    },
                    onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
                    {
                        list.list.Move(oldIndex, newIndex);
                        listProperties.Move(oldIndex, newIndex);
                    },
                    elementHeightCallback = i => (listProperties[i]?.GetPropertyHeight()) ?? EditorGUIUtility.singleLineHeight,
                    onCanAddCallback = x => property.canRead,
                    onCanRemoveCallback = x => property.canRead && x.count > 0
                };
            }
        }
    }
}
