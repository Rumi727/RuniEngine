#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Install
{
    public sealed class WelcomeScreen : IInstallerScreen
    {
        public string label { get; } = "Welcome!";
        public bool headDisable { get; } = true;

        public int sort { get; } = 0;

        static GUIStyle? headStyle;
        public void DrawGUI()
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            headStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 30 };
            GUILayout.Label("환영합니다!", headStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.Label("Runi Engine을 사용해 주셔서 감사합니다.");
            GUILayout.Label("지금부터 엔진의 초기 설정을 시작합니다.");
        }
    }
}
