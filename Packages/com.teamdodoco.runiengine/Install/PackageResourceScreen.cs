#nullable enable
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Install
{
    public sealed class PackageResourceScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label { get; } = "기본 리소스 복사";
        public bool headDisable { get; } = false;
        public int sort { get; } = 3;

        const string assetsResourcePathParent = "Assets/Runi Engine/Resources";
        const string packagePathParent = "Packages/com.teamdodoco.runiengine/Package Resources";

        public void DrawGUI()
        {
            if (Directory.Exists(packagePathParent))
            {
                PackageImportButton("폰트 리소스", "Fonts");
                return;
            }

            EditorGUILayout.HelpBox("패키지 리소스를 찾을 수 없습니다!", MessageType.Error);
        }

        public void PackageImportButton(string title, string packagePath)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(false));

            if (Directory.Exists(Path.Combine(assetsResourcePathParent, packagePath)))
            {
                if (GUILayout.Button("임포트 ✓", GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(Path.Combine(packagePathParent, packagePath + ".unitypackage"), false);
                    return;
                }
            }
            else
            {
                if (GUILayout.Button("임포트", GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(Path.Combine(packagePathParent, packagePath + ".unitypackage"), false);
                    return;
                }
            }

            GUILayout.EndHorizontal();
            return;
        }
    }
}
