#nullable enable
using RuniEngine.Settings;

namespace RuniEngine.Splashs
{
    public static class SplashScreen
    {
        [ProjectData]
        public struct ProjectData
        {
            public static string splashScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/SplashS/Default Splash Scene.unity";
            //public static string sceneLoadingScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/Scene Management/Scene Load Scene.unity";

            public static int startSceneIndex { get; set; } = -1;
        }

        [StaticResettable] public static bool isPlaying { get; set; } = false;
        [StaticResettable] public static bool resourceLoadable { get; set; } = false;
    }
}
