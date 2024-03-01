#nullable enable
using RuniEngine.Datas;
using RuniEngine.Splashs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace RuniEngine.Editor
{
    [InitializeOnLoad]
    public static class BuildSceneListChanger
    {
        public static string splashScenePath => SplashScreen.ProjectData.splashScenePath;
        //public static string sceneLoadingScenePath => SplashScreen.ProjectData.sceneLoadingScenePath;

        static BuildSceneListChanger()
        {
            EditorBuildSettings.sceneListChanged += () => SceneListChanged(true);
            SceneListChanged(true);
        }

        static bool sceneListChangedDisable = false;
        static StorableClass? splashScreen = null;
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
                }

                List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
                for (int i = 0; i < buildScenes.Count; i++)
                {
                    EditorBuildSettingsScene scene = buildScenes[i];
                    if (splashScenePath == scene.path)
                    {
                        buildScenes.RemoveAt(i);
                        i--;
                    }
                }

                buildScenes.Insert(0, new EditorBuildSettingsScene() { path = splashScenePath, enabled = true });
                //buildScenes.Insert(1, new EditorBuildSettingsScene() { path = sceneLoadingScenePath, enabled = true });

                EditorBuildSettings.scenes = buildScenes.ToArray();
            }
            finally
            {
                sceneListChangedDisable = false;
            }
        }
    }
}
