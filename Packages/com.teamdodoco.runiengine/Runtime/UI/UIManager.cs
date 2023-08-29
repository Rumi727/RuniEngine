#nullable enable
using Newtonsoft.Json;
using RuniEngine.Data;
using UnityEngine;

namespace RuniEngine.UI
{
    public static class UIManager
    {
        [UserData]
        public struct UserData
        {
            [JsonProperty] public static Color mainColor { get; set; } = new Color32(255, 15, 70, 255);
            [JsonProperty] public static float defaultLerpAniSpeed { get; set; } = 0.2f; 
        }
    }
}
