using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void DrawLine(int thickness = 1, int padding = 10) => DrawLine(new Color(0.4980392f, 0.4980392f, 0.4980392f), thickness, padding);

        public static void DrawLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding - 2));
            r.height = thickness;
            r.y += padding / 2 - 2;
            r.x -= 18;
            r.width += 22;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawLineVertical(int thickness = 1, int padding = 10) => DrawLineVertical(new Color(0.4980392f, 0.4980392f, 0.4980392f), thickness, padding);

        public static void DrawLineVertical(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding - 2));
            r.width = thickness;
            r.x += padding / 2 - 2;
            r.y -= 18;
            r.height += 22;
            EditorGUI.DrawRect(r, color);
        }
    }
}
