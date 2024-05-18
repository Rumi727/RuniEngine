#nullable enable
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor
{
    public abstract class RuniDictionaryTreeView : RuniTreeView<RuniDictionaryTreeViewItem>
    {
        public RuniDictionaryTreeView(TreeViewState state) : base(state) { }
        public RuniDictionaryTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) { }




        public bool ContainsKey(string key) => items.ContainsKey(key);
        public RuniDictionaryTreeViewItem FindItem(string key) => items[key];

        public void SetSelection(string key) => SetSelection(FindItem(key).id);

        Dictionary<string, RuniDictionaryTreeViewItem> items { get; } = new Dictionary<string, RuniDictionaryTreeViewItem>();
        public RuniDictionaryTreeViewItem CreateDictionaryTree(RuniDictionaryTreeViewItem root, IList<string> keyList, char separator = '.')
        {
            items.Clear();

            int id = 1;
            for (int i = 0; i < keyList.Count; i++)
            {
                string key = keyList[i];
                string[] keySplits = key.Split(separator);

                string comparisonKey = "";
                for (int j = 0; j < keySplits.Length; j++)
                {
                    string keySplit = keySplits[j];
                    string comparisonParentKey = comparisonKey;

                    if (j > 0)
                        comparisonKey += "." + keySplit;
                    else
                        comparisonKey += keySplit;

                    if (!items.ContainsKey(comparisonKey))
                    {
                        if (j > 0)
                        {
                            RuniDictionaryTreeViewItem item = new RuniDictionaryTreeViewItem(id, comparisonKey, keySplit);

                            items[comparisonParentKey].AddChild(item);
                            items.Add(comparisonKey, item);

                            id++;
                        }
                        else
                        {
                            RuniDictionaryTreeViewItem item = new RuniDictionaryTreeViewItem(id, comparisonKey, keySplit);

                            items.Add(comparisonKey, item);
                            root.AddChild(item);

                            id++;
                        }
                    }
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
    }
}
