#nullable enable
using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.TypeDrawers
{
    public abstract class ListTypeDrawerBase : TypeDrawer
    {
        protected ListTypeDrawerBase(SerializedTypeProperty property) : base(property) { }

        readonly AnimFloat animFloat = new AnimFloat(0);

        protected ReorderableList reorderableList
        {
            get
            {
                IList? list = GetList();
                if (list == null)
                    return new ReorderableList(Array.Empty<object>(), typeof(object));

                if (_reorderableList != null)
                    _reorderableList.list = list;

                return _reorderableList ??= new ReorderableList(list, GetListType(list))
                {
                    multiSelect = true,
                    headerHeight = 0,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        if (reorderableList == null)
                            return;

                        rect.x += 8;
                        rect.width -= 8;
                        rect.y += 1;
                        rect.height -= 2;

                        BeginMinLabelWidth(0, rect.width + 11, 0);
                        DrawElementCallback(rect, index, isActive, isFocused);
                        EndLabelWidth();
                    },
                    elementHeightCallback = i => ElementHeightCallback(reorderableList?.list ?? Array.Empty<object>(), i),
                    onAddCallback = x =>
                    {
                        x.GrabKeyboardFocus();
                        x.index = OnAddCallback(x);
                    },
                    onRemoveCallback = OnRemoveCallback,
                    onReorderCallbackWithDetails = OnReorderCallbackWithDetails,
                    onCanAddCallback = CanAddCallback,
                    onCanRemoveCallback = CanRemoveCallback
                };
            }
        }
        ReorderableList? _reorderableList;


        protected override void InternalOnGUI(Rect position, GUIContent? label)
        {
            float headHeight = GetYSize(EditorStyles.foldoutHeader);
            Rect headPosition = position;
            headPosition.height = headHeight;

            {
                if (property.DrawNullableButton(headPosition, label, out bool isDrawed))
                {
                    animFloat.target = 0;
                    if (animFloat.isAnimating)
                        RepaintCurrentWindow();

                    return;
                }
                else if (isDrawed)
                    headPosition.width -= 42;
            }

            if (!property.canRead)
            {
                base.InternalOnGUI(headPosition, label);
                return;
            }

            IList? list = GetList();
            if (list == null || GetListType(list) == typeof(object))
            {
                base.InternalOnGUI(headPosition, label);
                return;
            }

            property.isExpanded = ListHeader(headPosition, GetLists(), label ?? GUIContent.none, property.isExpanded, HeaderAddAction, HeaderRemoveAction, property.isInArray);

            position.y += headHeight;

            if (!property.isInArray)
            {
                float height;

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
                reorderableList.DoList(position);
        }

        public override float GetPropertyHeight()
        {
            if (!property.canRead)
                return base.GetPropertyHeight();

            IList? list = GetList();
            if (list == null)
                return base.GetPropertyHeight() + animFloat.value;

            float headerHeight = GetYSize(EditorStyles.foldoutHeader);
            float height;

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

        public abstract SerializedTypeProperty? GetSerializedTypeProperty(IList list, int index);

        public abstract IEnumerable<IList>? GetLists();

        public IList? GetList() => GetLists()?.MinBy(x => x.Count);

        public virtual float ElementHeightCallback(IList list, int index) => (GetSerializedTypeProperty(list, index)?.GetPropertyHeight()) ?? EditorGUIUtility.singleLineHeight;

        public abstract void HeaderAddAction(IList list, int listIndex, int index);
        public abstract void HeaderRemoveAction(IList list, int listIndex, int index);

        public abstract void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused);

        public abstract int OnAddCallback(ReorderableList list);

        public abstract void OnRemoveCallback(ReorderableList list);

        public abstract void OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex);

        public virtual bool CanAddCallback(ReorderableList list) => property.canRead;
        public virtual bool CanRemoveCallback(ReorderableList list) => property.canRead && list.count > 0;
    }
}
