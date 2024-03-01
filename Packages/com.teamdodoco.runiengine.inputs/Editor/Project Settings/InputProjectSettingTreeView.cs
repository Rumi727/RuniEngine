#nullable enable
using RuniEngine.Inputs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace RuniEngine.Editor.ProjectSettings
{
    public class InputProjectSettingTreeView : RuniTreeView<InputProjectSettingTreeViewItem>
    {
        public InputProjectSettingTreeView(TreeViewState state) : base(state) { }

        public Dictionary<string, int> itemIDs { get; } = new Dictionary<string, int>();



        protected override TreeViewItem BuildRoot()
        {
            InputProjectSettingTreeViewItem root = new InputProjectSettingTreeViewItem(0, -1, "", "root");

            List<KeyValuePair<string, KeyCode[]>> controls = InputManager.ProjectData.controlList.ToList();
            Dictionary<string, InputProjectSettingTreeViewItem> items = new Dictionary<string, InputProjectSettingTreeViewItem>();

            itemIDs.Clear();

            int id = 1;
            for (int i = 0; i < controls.Count; i++)
            {
                KeyValuePair<string, KeyCode[]> pair = controls[i];
                string[] keySplit = pair.Key.Split('.');

                string allKey = "";
                for (int j = 0; j < keySplit.Length; j++)
                {
                    string key = keySplit[j];
                    string parentAllKey = allKey;

                    if (j > 0)
                        allKey += "." + key;
                    else
                        allKey += key;

                    if (!items.ContainsKey(allKey))
                    {
                        if (j > 0)
                        {
                            InputProjectSettingTreeViewItem item = new InputProjectSettingTreeViewItem(id, allKey, key);

                            items[parentAllKey].AddChild(item);
                            items.Add(allKey, item);

                            itemIDs.Add(allKey, id);
                            id++;
                        }
                        else
                        {
                            InputProjectSettingTreeViewItem item = new InputProjectSettingTreeViewItem(id, allKey, key);

                            items.Add(allKey, item);
                            root.AddChild(item);

                            itemIDs.Add(allKey, id);
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
