using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ControlPanels
{
    public sealed class ControlPanelMainWindow : EditorWindow
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            controlPanels.Clear();

            IReadOnlyList<Type> types = ReflectionManager.types;
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                if (type.IsSubtypeOf<IControlPanelWindow>())
                    controlPanels.Add((IControlPanelWindow)Activator.CreateInstance(type));
            }

            controlPanels = controlPanels.OrderBy(x => x.sort).ToList();
            tabLabels = new string[controlPanels.Count];
        }

        [MenuItem("Runi Engine/Control Panel")]
        public static void ShowWindow()
        {
            if (!HasOpenInstances<ControlPanelMainWindow>())
                GetWindow<ControlPanelMainWindow>(false, "Runi Engine");
            else
                FocusWindowIfItsOpen<ControlPanelMainWindow>();
        }

        void OnGUI() => DrawGUI();

        void Update()
        {
            if (selectedControlPanel == null)
                return;

            if ((Kernel.isPlaying || selectedControlPanel.allowEditorUpdate) && !inspectorUpdate && selectedControlPanel.allowUpdate)
                Repaint();
        }

        void OnInspectorUpdate()
        {
            if (selectedControlPanel == null)
                return;

            if ((Kernel.isPlaying || selectedControlPanel.allowEditorUpdate) && inspectorUpdate && selectedControlPanel.allowUpdate)
                Repaint();
        }



        static bool inspectorUpdate = true;
        static int tabIndex = 0;
        static List<IControlPanelWindow> controlPanels = new List<IControlPanelWindow>();
        static string[] tabLabels = new string[0];
        static Vector2 scrollPosition = Vector2.zero;
        static IControlPanelWindow? selectedControlPanel;
        public static void DrawGUI()
        {
            if (controlPanels.Count <= 0)
                return;

            selectedControlPanel = controlPanels[tabIndex];
            {
                Space();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                for (int i = 0; i < controlPanels.Count; i++)
                    tabLabels[i] = TryGetText(controlPanels[i].label);

                tabIndex = GUILayout.Toolbar(tabIndex, tabLabels, GUILayout.ExpandWidth(false));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                Space(5);

                if ((Kernel.isPlayingAndNotPaused || selectedControlPanel.allowEditorUpdate) && selectedControlPanel.allowUpdate)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(TryGetText("control_panel.refresh_delay"), GUILayout.ExpandWidth(false));
                    inspectorUpdate = EditorGUILayout.Toggle(inspectorUpdate, GUILayout.Width(15));

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                DrawLine(2);
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selectedControlPanel.OnGUI();
            GUILayout.EndScrollView();
        }
    }
}
