#nullable enable
using RuniEngine.Data;
using RuniEngine.Splash;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace RuniEngine.Editor.Scene
{
    [InitializeOnLoad]
    public static class BuildSceneListChange
    {
        public static string splashScenePath => SplashScreen.ProjectData.splashScenePath;

        static BuildSceneListChange()
        {
            EditorBuildSettings.sceneListChanged += () => SceneListChanged(true);
            SceneListChanged(true);
        }

        static bool sceneListChangedDisable = false;
        static StorableClass? storableClass = null;
        public static void SceneListChanged(bool loadData)
        {
            if (Kernel.isPlaying)
                return;
            else if (sceneListChangedDisable)
                return;

            try
            {
                sceneListChangedDisable = true;

                if (loadData)
                {
                    storableClass ??= new StorableClass(typeof(SplashScreen.ProjectData));
                    storableClass.AutoNameLoad(Kernel.projectDataPath);
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
                EditorBuildSettings.scenes = buildScenes.ToArray();
            }
            finally
            {
                sceneListChangedDisable = false;
            }
        }
    }
}
