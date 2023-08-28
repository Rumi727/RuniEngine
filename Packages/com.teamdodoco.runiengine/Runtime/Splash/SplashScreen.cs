#nullable enable
using Newtonsoft.Json;
using RuniEngine.Data;

namespace RuniEngine.Splash
{
    public static class SplashScreen
    {
        [ProjectData]
        public sealed class ProjectData
        {
            [JsonProperty] public static string splashScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/Splash/Default Splash Scene.unity";
            [JsonProperty] public static int startSceneIndex { get; set; } = -1;
        }

        public static bool isPlaying { get; set; } = false;
    }
}
