#nullable enable
using Newtonsoft.Json;
using RuniEngine.Datas;

namespace RuniEngine.Splashs
{
    public static class SplashScreen
    {
        [ProjectData]
        public struct ProjectData
        {
            [JsonProperty] public static string splashScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/SplashS/Default Splash Scene.unity";
            //[JsonProperty] public static string sceneLoadingScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/Scene Management/Scene Load Scene.unity";

            [JsonProperty] public static int startSceneIndex { get; set; } = -1;
        }

        public static bool isPlaying { get; set; } = false;
    }
}
