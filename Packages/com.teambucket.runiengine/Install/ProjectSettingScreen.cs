#nullable enable
using System;
using UnityEngine;
using UnityEngine.Video;

namespace RuniEngine.Install
{
    public sealed class ProjectSettingScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label { get; } = "프로젝트 설정 변경";
        public bool headDisable { get; } = false;

        public int sort { get; } = 1;

        public void DrawGUI()
        {
            AudioListener.volume = SettingChangeButton("오디오 설정의 기본 전역 볼륨 변경", AudioListener.volume, 0.5f);

            {
                AudioConfiguration config = AudioSettings.GetConfiguration();
                SettingChangeButton("오디오 설정의 최대 가상 | 실제 음성을 최고치로 변경", () =>
                {
                    config.numVirtualVoices = 4095;
                    config.numRealVoices = 255;

                    AudioSettings.Reset(config);

                    VideoPlayer? videoPlayer = InstallerMainWindow.videoPlayer;
                    if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
                    {
                        videoPlayer.Stop();
                        videoPlayer.Play();
                    }

                }, (config.numVirtualVoices, 4095), (config.numRealVoices, 255));
            }
        }

        public T? SettingChangeButton<T>(string title, T? currnetValue, T? changeValue, Action? changedAction = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(false));

            if (Equals(currnetValue, changeValue))
            {
                if (GUILayout.Button("변경 ✓", GUILayout.ExpandWidth(false)))
                {
                    changedAction?.Invoke();
                    return changeValue;
                }
            }
            else
            {
                if (GUILayout.Button("변경", GUILayout.ExpandWidth(false)))
                {
                    changedAction?.Invoke();
                    return changeValue;
                }
            }

            GUILayout.EndHorizontal();
            return currnetValue;
        }

        public void SettingChangeButton(string title, Action changedAction, params (object? currnetValue, object? changeValue)[] value)
        {
            bool equals = true;
            for (int i = 0; i < value.Length; i++)
            {
                (object? currnetValue, object? changeValue) = value[i];
                if (!Equals(currnetValue, changeValue))
                    equals = false;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(false));

            if (equals)
            {
                if (GUILayout.Button("변경 ✓", GUILayout.ExpandWidth(false)))
                    changedAction.Invoke();
            }
            else
            {
                if (GUILayout.Button("변경", GUILayout.ExpandWidth(false)))
                    changedAction.Invoke();
            }

            GUILayout.EndHorizontal();
        }
    }
}
