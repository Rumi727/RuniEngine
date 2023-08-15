#nullable enable
using UnityEngine;
using UnityEngine.Video;

namespace RuniEngine.Install
{
    public sealed class ProjectSettingScreen : IInstallerScreen
    {
        public string label { get; } = "프로젝트 설정 변경";
        public bool headDisable { get; } = false;

        public int sort { get; } = 1;

        public void DrawGUI()
        {
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("오디오 설정의 기본 전역 볼륨 변경", GUILayout.ExpandWidth(false));

                if (GUILayout.Button("변경", GUILayout.ExpandWidth(false)))
                    AudioListener.volume = 0.5f;

                GUILayout.EndHorizontal();
            }

            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("오디오 설정의 최대 가상 | 실제 음성을 최고치로 변경", GUILayout.ExpandWidth(false));

                if (GUILayout.Button("변경", GUILayout.ExpandWidth(false)))
                {
                    AudioConfiguration config = AudioSettings.GetConfiguration();
                    config.numVirtualVoices = 4095;
                    config.numRealVoices = 255;

                    AudioSettings.Reset(config);

                    VideoPlayer? videoPlayer = InstallerMainWindow.videoPlayer;
                    if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
                    {
                        videoPlayer.Stop();
                        videoPlayer.Play();
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}
