#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public sealed class ControlPanelMainWindow : EditorWindow
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            controlPanels.Clear();

            Type[] types = ReflectionManager.types;
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.IsSubtypeOf<IControlPanelWindow>())
                    controlPanels.Add((IControlPanelWindow)Activator.CreateInstance(type));
            }

            controlPanels = controlPanels.OrderBy(x => x.sort).ToList();

            tabLabels.Clear();
            for (int i = 0; i < controlPanels.Count; i++)
                tabLabels.Add(TryGetText(controlPanels[i].label));
        }

        [MenuItem("Runi Engine/Control Panel")]
        public static void ShowWindow()
        {
            if (!HasOpenInstances<ControlPanelMainWindow>())
                GetWindow<ControlPanelMainWindow>(true, "Runi Engine");
            else
                FocusWindowIfItsOpen<ControlPanelMainWindow>();
        }

        void OnGUI() => DrawGUI();

        void Update()
        {
            if (Kernel.isPlaying && !inspectorUpdate)
                Repaint();
        }

        void OnInspectorUpdate()
        {
            if (Kernel.isPlaying && inspectorUpdate)
                Repaint();
        }



        static bool inspectorUpdate = true;
        static int tabIndex = 0;
        static List<IControlPanelWindow> controlPanels = new List<IControlPanelWindow>();
        static readonly List<string> tabLabels = new List<string>();
        static Vector2 scrollPosition = Vector2.zero;
        public static void DrawGUI()
        {
            {
                Space();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                tabIndex = GUILayout.Toolbar(tabIndex, tabLabels.ToArray(), GUILayout.ExpandWidth(false));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                Space(5);

                if (Kernel.isPlayingAndNotPaused)
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

            if (controlPanels.Count > 0)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                controlPanels[tabIndex].OnGUI();
                GUILayout.EndScrollView();
            }
        }
    }
}
