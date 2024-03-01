#nullable enable
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor
{
    public class RuniDictionaryTreeViewItem : TreeViewItem
    {
        public RuniDictionaryTreeViewItem(int id, string key, string displayName) : base(id, 0, displayName) => this.key = key;
        public RuniDictionaryTreeViewItem(int id, int depth, string key, string displayName) : base(id, depth, displayName) => this.key = key;

        public string key { get; }
    }
}
