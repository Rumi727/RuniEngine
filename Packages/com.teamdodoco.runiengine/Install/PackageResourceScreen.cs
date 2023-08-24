#nullable enable
using RuniEngine.Resource;
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
            PackageImportButton("스트리밍 에셋", Kernel.streamingAssetsFolderName, Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName));
            PackageImportButton("폰트", "Fonts", Path.Combine(assetsResourcePathParent, "Fonts"));
        }

        public void PackageImportButton(string title, string packagePath, string existsPath)
        {
            string allPackagePath = Path.Combine(packagePathParent, packagePath + ".unitypackage");
            if (!File.Exists(allPackagePath))
            {
                EditorGUILayout.HelpBox($"{title} 패키지 리소스를 찾을 수 없습니다!", MessageType.Error);
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{title} 패키지 리소스", GUILayout.ExpandWidth(false));

            if (Directory.Exists(existsPath))
            {
                if (GUILayout.Button("임포트 ✓", GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(allPackagePath, false);
                    GUILayout.EndHorizontal();

                    return;
                }
            }
            else
            {
                if (GUILayout.Button("임포트", GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(allPackagePath, false);
                    GUILayout.EndHorizontal();

                    return;
                }
            }

            GUILayout.EndHorizontal();
            return;
        }
    }
}
