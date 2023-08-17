#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace RuniEngine.Install
{
    public sealed class InstallerMainWindow : EditorWindow
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            {
                installerScreens.Clear();

                Type[] types = ReflectionManager.types;
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (type != typeof(IInstallerScreen) && typeof(IInstallerScreen).IsAssignableFrom(type))
                        installerScreens.Add((IInstallerScreen)Activator.CreateInstance(type));
                }

                installerScreens = installerScreens.OrderBy(x => x.sort).ToList();
            }

            ShowInstallerWindow();
        }

        void OnEnable()
        {
            scrollPosition = Vector2.zero;
            screenIndex = 0;
            musicVolume = 0.7f;
        }

        void Update()
        {
            if (Kernel.isPlaying)
                return;

            if (videoPlayer != null)
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
            else
                MusicPlay().Forget();
        }

        void OnGUI() => DrawGUI();

        void OnDisable() => VideoDestroy();



        [MenuItem("Runi Engine/Show Installer")]
        public static void ShowInstallerWindow()
        {
            if (!HasOpenInstances<InstallerMainWindow>())
                GetWindow<InstallerMainWindow>(true, "Runi Engine Installer");
            else
                FocusWindowIfItsOpen<InstallerMainWindow>();
        }

        const string music = "https://youtu.be/G0WZTlLJlHg";
        public static VideoPlayer? videoPlayer;
        async UniTaskVoid MusicPlay()
        {
            VideoDestroy();
            Repaint();

            videoPlayer = Instantiate(ResourceManager.emptyTransform.gameObject, null).AddComponent<VideoPlayer>();
            videoPlayer.name = $"{nameof(InstallerMainWindow)} Background Music";

            string url = await YoutubePlayer.YoutubePlayer.GetRawVideoUrlAsync(music);

            if (videoPlayer == null)
                return;

            videoPlayer.isLooping = true;
            videoPlayer.url = url;

            videoPlayer.Play();
            videoPlayer.SetDirectAudioVolume(0, musicVolume);

            Repaint();
        }

        static void VideoDestroy()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                DestroyImmediate(videoPlayer.gameObject);
            }
        }

        static List<IInstallerScreen> installerScreens = new List<IInstallerScreen>();
        static GUIStyle? headStyle;

        static Vector2 scrollPosition = Vector2.zero;
        static int screenIndex = 0;
        static float musicVolume = 0.7f;

        public static void DrawGUI()
        {
            GUILayout.Space(3);
            headStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 18 };

            //Screen
            {
                if (screenIndex.Clamp(0) < installerScreens.Count)
                {
                    IInstallerScreen screen = installerScreens[screenIndex];

                    if (!screen.headDisable)
                    {
                        GUILayout.Label(screen.label, headStyle);
                        EditorTool.DrawLine(2, 0);
                    }

                    {
                        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                        screen.DrawGUI();

                        GUILayout.FlexibleSpace();
                        GUILayout.EndScrollView();
                    }
                }
            }

            //Button
            {
                EditorTool.DrawLine(2, 0);
                GUILayout.Space(4);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                {
                    {
                        GUI.enabled = screenIndex > 0;

                        if (GUILayout.Button("<"))
                        {
                            screenIndex--;
                            scrollPosition = Vector2.zero;
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.Label($"{screenIndex + 1}/{installerScreens.Count}");

                    {
                        GUI.enabled = screenIndex < installerScreens.Count - 1;

                        if (GUILayout.Button(">"))
                        {
                            screenIndex++;
                            scrollPosition = Vector2.zero;
                        }

                        GUI.enabled = true;
                    }

                    if (!Kernel.isPlaying)
                    {
                        GUILayout.Space(5);

                        if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
                        {
                            {
                                GUILayout.Label($"볼륨 {(musicVolume * 100).Floor()}/100");
                                float volume = GUILayout.HorizontalSlider(musicVolume, 0, 1, GUILayout.Width(100));

                                if (musicVolume != volume)
                                {
                                    musicVolume = volume;
                                    videoPlayer.SetDirectAudioVolume(0, musicVolume);
                                }
                            }

                            GUILayout.Space(10);

                            if (GUILayout.Button("오디오 리셋"))
                            {
                                AudioSettings.Reset(AudioSettings.GetConfiguration());

                                videoPlayer.Stop();
                                videoPlayer.Play();
                            }
                        }
                        else
                            GUILayout.Label("음악 로딩중...");
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
            }
        }
    }
}
