#nullable enable
using RuniEngine.Account;
using RuniEngine.Data;
using RuniEngine.Splash;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace RuniEngine.Editor.Scene
{
    [InitializeOnLoad]
    public static class BuildSceneListChanger
    {
        public static string splashScenePath => SplashScreen.ProjectData.splashScenePath;
        public static string loginScenePath => UserAccountManager.ProjectData.loginScenePath;

        static BuildSceneListChanger()
        {
            EditorBuildSettings.sceneListChanged += () => SceneListChanged(true);
            SceneListChanged(true);
        }

        static bool sceneListChangedDisable = false;
        static StorableClass? splashScreen = null;
        static StorableClass? userAccountManager = null;
        public static void SceneListChanged(bool loadData)
        {
            if (sceneListChangedDisable || Kernel.isPlaying)
                return;

            try
            {
                sceneListChangedDisable = true;

                if (loadData)
                {
                    splashScreen ??= new StorableClass(typeof(SplashScreen.ProjectData));
                    splashScreen.AutoNameLoad(Kernel.projectDataPath);

                    userAccountManager ??= new StorableClass(typeof(UserAccountManager.ProjectData));
                    userAccountManager.AutoNameLoad(Kernel.projectDataPath);
                }

                List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
                for (int i = 0; i < buildScenes.Count; i++)
                {
                    EditorBuildSettingsScene scene = buildScenes[i];
                    if (splashScenePath == scene.path || loginScenePath == scene.path)
                    {
                        buildScenes.RemoveAt(i);
                        i--;
                    }
                }

                buildScenes.Insert(0, new EditorBuildSettingsScene() { path = splashScenePath, enabled = true });
                buildScenes.Insert(1, new EditorBuildSettingsScene() { path = loginScenePath, enabled = true });

                EditorBuildSettings.scenes = buildScenes.ToArray();
            }
            finally
            {
                sceneListChangedDisable = false;
            }
        }
    }
}
