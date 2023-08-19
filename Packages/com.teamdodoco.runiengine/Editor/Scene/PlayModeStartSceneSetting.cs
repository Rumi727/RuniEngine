#nullable enable
using UnityEditor.SceneManagement;
using UnityEditor;
using RuniEngine.Data;
using RuniEngine.Splash;

namespace RuniEngine.Editor
{
    [InitializeOnLoad]
    public static class PlayModeStartSceneSetting
    {
        static PlayModeStartSceneSetting() => SetPlayModeStartScene(true);

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
