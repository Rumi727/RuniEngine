#nullable enable
using RuniEngine.Inputs;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor.ProjectSettings
{
    public class InputProjectSettingTreeView : RuniDictionaryTreeView
    {
        public InputProjectSettingTreeView(TreeViewState state) : base(state) { }

        protected override TreeViewItem BuildRoot() => CreateDictionaryTree(new RuniDictionaryTreeViewItem(0, -1, "", "root"), InputManager.ProjectData.controlList.Keys.ToArray());
    }
}
