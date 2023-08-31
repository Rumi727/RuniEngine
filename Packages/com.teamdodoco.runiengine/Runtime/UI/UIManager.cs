#nullable enable
using Newtonsoft.Json;
using RuniEngine.Data;
using RuniEngine.Json;
using UnityEngine;

namespace RuniEngine.UI
{
    public static class UIManager
    {
        [UserData]
        public struct UserData
        {
            [JsonProperty] public static JColor mainColor { get; set; } = new Color32(255, 15, 70, 255);
            [JsonProperty] public static float defaultLerpAniSpeed { get; set; } = 0.05f; 
        }
    }
}
