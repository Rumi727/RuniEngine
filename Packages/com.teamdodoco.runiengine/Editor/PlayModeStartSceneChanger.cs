using RuniEngine.Datas;
using RuniEngine.Splashs;
using UnityEditor;
using UnityEditor.SceneManagement;

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
                storableClass.AutoNameLoad(Kernel.projectSettingPath);
            }

            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SplashScreen.ProjectData.splashScenePath);
            if (EditorSceneManager.playModeStartScene != scene)
            {
                Debug.Log(EditorTool.TryGetText("internal.auto_setter.property.info").Replace("{name}", $"{nameof(EditorSceneManager)}.{nameof(EditorSceneManager.playModeStartScene)}"));
                EditorSceneManager.playModeStartScene = scene;
            }
        }
    }
}
