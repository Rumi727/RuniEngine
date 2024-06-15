#nullable enable
using UnityEditor.IMGUI.Controls;

namespace RuniEngine.Editor
{
    public class RuniAdvancedDropdownItem : AdvancedDropdownItem
    {
        public string path { get; }
        public int index { get; }

        public RuniAdvancedDropdownItem(string path, string name, int index) : base(name)
        {
            this.path = path;
            this.index = index;
        }
    }
}
