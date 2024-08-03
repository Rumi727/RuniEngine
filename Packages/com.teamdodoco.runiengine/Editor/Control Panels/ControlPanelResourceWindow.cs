#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Resource;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ControlPanels
{
    public class ControlPanelResourceWindow : IControlPanelWindow
    {
        public string label => "control_panel.resource";
        public int sort => 300;

        public bool allowUpdate => true;
        public bool allowEditorUpdate => false;

        public void OnGUI() => DrawGUI();

        static readonly ReorderableList? reorderableList;
        public static void DrawGUI()
        {
            EditorGUI.BeginDisabledGroup(!Kernel.isPlaying);

            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(TryGetText("control_panel.resource.refresh"), GUILayout.ExpandWidth(false)))
                    ResourceManager.Refresh().Forget();

                GUILayout.EndHorizontal();
            }

            {
                /*List<string> packLists = ResourceManager.GlobalData.resourcePacks;
                float height = EditorStyles.textField.CalcSize(new GUIContent()).y;
                //bool isChanged = false;

                reorderableList ??= new ReorderableList(packLists, typeof(List<string>), true, true, true, true)
                {
                    drawHeaderCallback = static (Rect rect) => GUI.Label(rect, TryGetText("control_panel.resource.resource_pack")),
                    elementHeight = (height + 4f),
                    multiSelect = true
                };

                reorderableList.list = packLists;

                reorderableList.onAddCallback = x =>
                {
                    packLists.Add("");
                    //isChanged = true;
                };

                reorderableList.onRemoveCallback = x =>
                {
                    if (x.selectedIndices.Count > 0)
                    {
                        for (int i = 0; i < x.selectedIndices.Count; i++)
                        {
                            int index = x.selectedIndices[i];
                            packLists.RemoveAt(index.Clamp(0, x.list.Count - 1));
                        }
                    }
                    else
                        packLists.RemoveAt(x.list.Count - 1);

                    //isChanged = true;
                };

                //reorderableList.onReorderCallback = x => isChanged = true;

                reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 3;
                    rect.height = height;

                    packLists[index] = GUI.TextField(rect, packLists[index]);
                };

                Rect rect = EditorGUILayout.GetControlRect(true, reorderableList.GetHeight());
                reorderableList.DoList(rect);*/
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
