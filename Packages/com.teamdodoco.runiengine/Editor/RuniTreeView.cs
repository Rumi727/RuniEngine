#nullable enable
using System.Collections.Generic;
using System;
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor
{
    public abstract class RuniTreeView<T> : TreeView where T : TreeViewItem
    {
        public RuniTreeView(TreeViewState state) : base(state) { }

        public RuniTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) { }




        public new T rootItem => (T)base.rootItem;

        public T? selectedItem { get; private set; }
        public T?[] selectedItems { get; private set; } = new T[0];

        public event Action<IList<int>>? selectionChanged;



        public new void Reload()
        {
            base.Reload();
            SetSelectedItem(GetSelection());
        }

        protected override void SelectionChanged(IList<int> selectedIDs)
        {
            SetSelectedItem(selectedIDs);
            selectionChanged?.Invoke(selectedIDs);
        }

        void SetSelectedItem(IList<int> selectedIDs)
        {
            if (selectedIDs.Count == 1)
                selectedItem = (T)FindItem(selectedIDs[0], rootItem);
            else
                selectedItem = null;

            selectedItems = new T[selectedIDs.Count];
            for (int i = 0; i < selectedItems.Length; i++)
                selectedItems[i] = (T)FindItem(selectedIDs[i], rootItem);
        }

        readonly int[] setSelectionIDs = new int[] { 0 };
        public void SetSelection(int selectedID)
        {
            setSelectionIDs[0] = selectedID;
            SetSelection(setSelectionIDs);
        }

        public void SetSelection(params int[] selectedIDs) => SetSelection((IList<int>)selectedIDs);

        public T FindItem(int id) => (T)FindItem(id, rootItem);
    }
}
