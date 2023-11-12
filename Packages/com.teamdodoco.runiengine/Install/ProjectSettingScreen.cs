#nullable enable
using RuniEngine.Editor;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace RuniEngine.Install
{
    public sealed class ProjectSettingScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label => EditorTool.TryGetText("installer.project_setting.label");
        public bool headDisable { get; } = false;

        public int sort { get; } = 1;

        public void DrawGUI()
        {
            AudioListener.volume = SettingChangeButton("installer.project_setting.audio_volume", AudioListener.volume, 1f);

            {
                AudioConfiguration config = AudioSettings.GetConfiguration();
                SettingChangeButton("installer.project_setting.audio_max", () =>
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
            GUILayout.Label(EditorTool.TryGetText(title), GUILayout.ExpandWidth(false));

            if (Equals(currnetValue, changeValue))
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.project_setting.change_done"), GUILayout.ExpandWidth(false)))
                {
                    changedAction?.Invoke();
                    return changeValue;
                }
            }
            else
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.project_setting.change"), GUILayout.ExpandWidth(false)))
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
            GUILayout.Label(EditorTool.TryGetText(title), GUILayout.ExpandWidth(false));

            if (equals)
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.project_setting.change_done"), GUILayout.ExpandWidth(false)))
                    changedAction.Invoke();
            }
            else
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.project_setting.change"), GUILayout.ExpandWidth(false)))
                    changedAction.Invoke();
            }

            GUILayout.EndHorizontal();
        }
    }
}
