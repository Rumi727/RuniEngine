#nullable enable
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Install
{
    public sealed class WelcomeScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label => TryGetText("installer.welcome");
        public bool headDisable { get; } = true;

        public int sort { get; } = 0;

        static GUIStyle? headStyle;
        public void DrawGUI()
        {
            float timer = (float)InstallerMainWindow.stopwatch.Elapsed.TotalSeconds;
            GUILayout.FlexibleSpace();

            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(100));
                GUILayout.Space(20);

                {
                    GUILayout.BeginVertical();

                    headStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 30 };

                    {
                        Rect rect = EditorGUILayout.GetControlRect(false, 40);
                        Color preColor = GUI.contentColor;

                        Label("ko_kr", 0);
                        Label("en_us", 100);
                        Label("ja_jp", 50);

                        GUI.color = preColor;

                        void Label(string language, float yOffset)
                        {
                            Rect rect2 = new Rect(new Vector2(rect.x, (rect.y + (timer * 20) + yOffset).Repeat(150)), rect.size);
                            float dis = (rect2.y - rect.y);
                            float alpha;

                            if (dis < 0)
                                alpha = (1 - (dis.Abs() / 50)) * 2;
                            else
                                alpha = 1 - (dis.Abs() / 25);

                            GUI.color = new Color(preColor.r, preColor.g, preColor.b, alpha);
                            GUI.Label(rect2, TryGetText("installer.welcome", language), headStyle);
                        }
                    }

                    GUILayout.Space(20);

                    GUILayout.Label(TryGetText("installer.welcome.text"));
                    GUILayout.Label(TryGetText("installer.welcome.text2"));

                    GUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
        }
    }
}
