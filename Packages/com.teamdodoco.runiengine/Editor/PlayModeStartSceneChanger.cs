#nullable enable
using UnityEditor.SceneManagement;
using UnityEditor;
using RuniEngine.Datas;
using RuniEngine.Splashs;

namespace RuniEngine.Editor
{
    [InitializeOnLoad]
    public static class PlayModeStartSceneChanger
    {
        static PlayModeStartSceneChanger() => SetPlayModeStartScene(true);

        static StorableClass? storableClass = null;
        public static void SetPlayModeStartScene(bool loadData)
        {
            if (loadData)
            {
                storableClass ??= new StorableClass(typeof(SplashScreen.ProjectData));
                storableClass.AutoNameLoad(Kernel.projectDataPath);
            }

            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SplashScreen.ProjectData.splashScenePath);
        }
    }
}
