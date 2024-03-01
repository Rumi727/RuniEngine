#nullable enable
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor
{
    public class NBSProjectSettingTreeView : RuniDictionaryTreeView
    {
        public NBSProjectSettingTreeView(TreeViewState state) : base(state) { }

        public IList<string> keyList { get; set; } = new List<string>();

        protected override TreeViewItem BuildRoot() => CreateDictionaryTree(new RuniDictionaryTreeViewItem(0, -1, "", "root"), keyList);
    }
}
