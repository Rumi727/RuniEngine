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

            [JsonProperty] public static float defaultLerpAniSpeed { get => _defaultLerpAniSpeed.Clamp(0); set => _defaultLerpAniSpeed = value.Clamp(0); }
            static float _defaultLerpAniSpeed = 0.08f;

            [JsonProperty] public static float uiSize { get => _uiSize.Clamp(0); set => _uiSize = value.Clamp(0); }
            static float _uiSize = 1;
        }

        public static float uiSize { get => UserData.uiSize; set => UserData.uiSize = value; }
    }
}
