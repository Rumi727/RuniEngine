#nullable enable
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor.ProjectSettings
{
    public class InputProjectSettingTreeViewItem : TreeViewItem
    {
        public InputProjectSettingTreeViewItem(int id, string key, string displayName) : base(id, 0, displayName) => this.key = key;
        public InputProjectSettingTreeViewItem(int id, int depth, string key, string displayName) : base(id, depth, displayName) => this.key = key;

        public string key { get; }
    }
}
