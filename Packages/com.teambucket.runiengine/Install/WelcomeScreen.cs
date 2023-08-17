#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Install
{
    public sealed class WelcomeScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label { get; } = "Welcome!";
        public bool headDisable { get; } = true;

        public int sort { get; } = 0;

        float rotation = 0;
        static GUIStyle? headStyle;
        public void DrawGUI()
        {
            Texture2D? logo = null;
            float timer = 0;
            if (installerMainWindow != null)
            {
                logo = installerMainWindow.logoTexture;
                timer = (float)(installerMainWindow.stopwatch.Elapsed.TotalSeconds);

                installerMainWindow.Repaint();
            }

            GUILayout.FlexibleSpace();

            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                {
                    GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(100));
                    rotation = ((timer - timer.Repeat(0.1f)) * 32).Repeat(360);

                    Matrix4x4 matrix = GUI.matrix;
                    GUIUtility.RotateAroundPivot(rotation, rect.center);

                    GUI.DrawTexture(rect, logo);

                    GUI.matrix = matrix;
                    GUI.EndGroup();
                }

                GUILayout.Space(20);

                {
                    GUILayout.BeginVertical();

                    headStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 30 };
                    GUILayout.Label("환영합니다!", headStyle);

                    GUILayout.Space(20);

                    GUILayout.Label("Runi Engine을 사용해 주셔서 감사합니다.");
                    GUILayout.Label("지금부터 엔진의 초기 설정을 시작합니다.");

                    GUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
        }
    }
}
