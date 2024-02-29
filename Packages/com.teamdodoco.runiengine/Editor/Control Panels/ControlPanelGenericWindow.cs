#nullable enable
using RuniEngine.Threading;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;
using RuniEngine.Accounts;
using RuniEngine.Resource;

namespace RuniEngine.Editor
{
    public class ControlPanelGenericWindow : IControlPanelWindow
    {
        public string label { get; } = "control_panel.generic";
        public int sort { get; } = 0;

        public void OnGUI() => DrawGUI();

        public static void DrawGUI()
        {
            if (Kernel.isPlayingAndNotPaused)
            {
                DrawText("control_panel.generic.delta_time", Kernel.deltaTime);
                DrawText("control_panel.generic.smooth_delta_time", Kernel.smoothDeltaTime);
                DrawText("control_panel.generic.fps_delta_time", Kernel.fpsDeltaTime);
                DrawText("control_panel.generic.fps_smooth_delta_time", Kernel.fpsSmoothDeltaTime);
                DrawText("control_panel.generic.unscaled_delta_time", Kernel.unscaledDeltaTime);
                DrawText("control_panel.generic.unscaled_smooth_delta_time", Kernel.unscaledSmoothDeltaTime);
                DrawText("control_panel.generic.fps_unscaled_delta_time", Kernel.fpsUnscaledDeltaTime);
                DrawText("control_panel.generic.fps_unscaled_smooth_delta_time", Kernel.fpsUnscaledSmoothDeltaTime);

                DrawLine();

                DrawText("control_panel.generic.fps", Kernel.fps);

                DrawLine();

                DrawText("control_panel.generic.memory", (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4));

                DrawLine();

                DrawText("control_panel.generic.main_thread_id", ThreadManager.mainThreadId);

                DrawLine();

                DrawText("control_panel.generic.data_path", Kernel.dataPath);
                DrawText("control_panel.generic.streaming_assets_path", Kernel.streamingAssetsPath);
                DrawText("control_panel.generic.persistent_data_path", Kernel.persistentDataPath);
                DrawText("control_panel.generic.temporary_cache_path", Kernel.temporaryCachePath);
                DrawText("control_panel.generic.resource_pack_path", Kernel.resourcePackPath);
                DrawText("control_panel.generic.project_data_path", Kernel.projectDataPath);

                DrawLine();

                DrawText("control_panel.generic.company_name", Kernel.companyName);
                DrawText("control_panel.generic.product_name", Kernel.productName);

                DrawLine();

                {
                    string account_status;
                    if (UserAccountManager.currentAccount != null)
                        account_status = "control_panel.generic.account_login";
                    else
                        account_status = "control_panel.generic.account_logout";

                    DrawText("control_panel.generic.account_status", TryGetText(account_status));

                    if (UserAccountManager.currentAccount != null)
                    {
                        DrawText("control_panel.generic.account_name", UserAccountManager.currentAccount.name);
                        DrawText("control_panel.generic.account_path", UserAccountManager.currentAccount.path);
                        DrawText("control_panel.generic.account_hashed_password", UserAccountManager.currentAccount.hashedPassword);
                    }
                }

                DrawLine();

                DrawText("control_panel.generic.version", Kernel.version);
                DrawText("control_panel.generic.unity_version", Kernel.unityVersion);

                DrawLine();

                DrawText("control_panel.generic.platform", Kernel.platform);

                DrawLine();

                DrawText("control_panel.generic.operating_system", SystemInfo.operatingSystem);

                DrawLine();

                DrawText("control_panel.generic.device_model", SystemInfo.deviceModel);
                DrawText("control_panel.generic.device_name", SystemInfo.deviceName);

                DrawLine();

                DrawText("control_panel.generic.internet_reachability", Kernel.internetReachability);
                DrawText("control_panel.generic.battery_status", SystemInfo.batteryStatus);

                DrawLine();

                DrawText("control_panel.generic.processor_type", SystemInfo.processorType);
                DrawText("control_panel.generic.processor_frequency", SystemInfo.processorFrequency);
                DrawText("control_panel.generic.processor_count", SystemInfo.processorCount);

                DrawLine();

                DrawText("control_panel.generic.graphics_device_name", SystemInfo.graphicsDeviceName);
                DrawText("control_panel.generic.graphics_memory_size", SystemInfo.graphicsMemorySize);

                DrawLine();

                DrawText("control_panel.generic.system_memory_size", SystemInfo.systemMemorySize);

                DrawLine();

                Kernel.gameSpeed = EditorGUILayout.FloatField(TryGetText("control_panel.generic.game_speed"), Kernel.gameSpeed);
            }
            else
            {
                DrawText("control_panel.generic.memory", (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4));

                DrawLine();

                DrawText("control_panel.generic.main_thread_id", ThreadManager.mainThreadId);

                DrawLine();

                DrawText("control_panel.generic.data_path", Kernel.dataPath);
                DrawText("control_panel.generic.streaming_assets_path", Kernel.streamingAssetsPath);
                DrawText("control_panel.generic.persistent_data_path", Kernel.persistentDataPath);
                DrawText("control_panel.generic.temporary_cache_path", Kernel.temporaryCachePath);
                DrawText("control_panel.generic.resource_pack_path", Kernel.resourcePackPath);
                DrawText("control_panel.generic.project_data_path", Kernel.projectDataPath);

                DrawLine();

                DrawText("control_panel.generic.company_name", Kernel.companyName);
                DrawText("control_panel.generic.product_name", Kernel.productName);

                DrawLine();

                DrawText("control_panel.generic.version", Kernel.version);
                DrawText("control_panel.generic.unity_version", Kernel.unityVersion);

                DrawLine();

                DrawText("control_panel.generic.platform", Kernel.platform);

                DrawLine();

                DrawText("control_panel.generic.operating_system", SystemInfo.operatingSystem);

                DrawLine();

                DrawText("control_panel.generic.device_model", SystemInfo.deviceModel);
                DrawText("control_panel.generic.device_name", SystemInfo.deviceName);

                DrawLine();

                DrawText("control_panel.generic.battery_status", SystemInfo.batteryStatus);

                DrawLine();

                DrawText("control_panel.generic.processor_type", SystemInfo.processorType);
                DrawText("control_panel.generic.processor_frequency", SystemInfo.processorFrequency);
                DrawText("control_panel.generic.processor_count", SystemInfo.processorCount);

                DrawLine();

                DrawText("control_panel.generic.graphics_device_name", SystemInfo.graphicsDeviceName);
                DrawText("control_panel.generic.graphics_memory_size", SystemInfo.graphicsMemorySize);

                DrawLine();

                DrawText("control_panel.generic.system_memory_size", SystemInfo.systemMemorySize);
            }
        }

        static void DrawText(string key, object value) => GUILayout.Label(TryGetText(key) + " - " + value);
    }
}
