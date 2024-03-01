#nullable enable
using UnityEditor;
using UnityEngine.UIElements;
using RuniEngine.Datas;
using RuniEngine.Splashs;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public class SplashProjectSetting : SettingsProvider
    {
        public SplashProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new SplashProjectSetting("Runi Engine/Splash Setting", SettingsScope.Project);



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                splashProjectSetting ??= new StorableClass(typeof(SplashScreen.ProjectData));
                splashProjectSetting.AutoNameLoad(Kernel.projectDataPath);
            }
        }

        public override void OnGUI(string searchContext) => DrawGUI();

        public static StorableClass? splashProjectSetting = null;
        public static void DrawGUI()
        {
            //라벨 길이 설정 안하면 유니티 버그 때매 이상해짐
            BeginLabelWidth(0);

            EditorGUI.BeginChangeCheck();

            string path = SplashScreen.ProjectData.splashScenePath;
            FileObjectField<SceneAsset>(TryGetText("project_setting.splash.splash_scene"), ref path, out _);
            SplashScreen.ProjectData.splashScenePath = path;

            path = SplashScreen.ProjectData.sceneLoadingScenePath;
            FileObjectField<SceneAsset>(TryGetText("project_setting.splash.loading_scene"), ref path, out _);
            SplashScreen.ProjectData.sceneLoadingScenePath = path;

            EditorGUILayout.Space();

            int startSceneIndex = SplashScreen.ProjectData.startSceneIndex;
            if (EditorGUILayout.Toggle(TryGetText("project_setting.splash.start_scene_enable"), startSceneIndex >= 0))
                startSceneIndex = startSceneIndex.Clamp(0);
            else
                startSceneIndex = -1;

            if (startSceneIndex >= 0)
                startSceneIndex = EditorGUILayout.IntSlider(TryGetText("project_setting.splash.start_scene_index"), startSceneIndex, 2, EditorBuildSettings.scenes.Length - 1);

            SplashScreen.ProjectData.startSceneIndex = startSceneIndex;

            //EditorGUILayout.Space();

            //VirtualMachineDetector.Data.vmBan = EditorGUILayout.Toggle("가상머신 밴", VirtualMachineDetector.Data.vmBan);

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (!Kernel.isPlaying && EditorGUI.EndChangeCheck())
            {
                PlayModeStartSceneChanger.SetPlayModeStartScene(false);
                BuildSceneListChanger.SceneListChanged(false);

                splashProjectSetting?.AutoNameSave(Kernel.projectDataPath);
            }

            EndLabelWidth();
        }
    }
}
