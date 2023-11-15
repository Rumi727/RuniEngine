#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Data;
using RuniEngine.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace RuniEngine.Install
{
    public sealed class InstallerMainWindow : EditorWindow
    {
        public static VideoPlayer? videoPlayer;

        public Texture2D? logoTexture;
        public Stopwatch stopwatch = new Stopwatch();



        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            {
                installerScreens.Clear();

                Type[] types = ReflectionManager.types;
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (type.IsSubtypeOf<IInstallerScreen>())
                        installerScreens.Add((IInstallerScreen)Activator.CreateInstance(type));
                }

                installerScreens = installerScreens.OrderBy(x => x.sort).ToList();
            }

            string path = Path.Combine("Assets", "~RumiEngineInstalled~");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
                EditorApplication.update += Update;

                static void Update()
                {
                    ShowInstallerWindow();
                    EditorApplication.update -= Update;
                }
            }
        }



        void OnEnable()
        {
            stopwatch.Restart();

            minSize = new Vector2(584, 285);
            maxSize = minSize;

            scrollPosition = Vector2.zero;
            screenIndex = 0;
            musicVolume = 0.5f;

            for (int i = 0; i < installerScreens.Count; i++)
                installerScreens[i].installerMainWindow = this;
        }

        void OnDisable() => VideoDestroy();

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



        [MenuItem("Runi Engine/Show Installer")]
        public static void ShowInstallerWindow()
        {
            if (!HasOpenInstances<InstallerMainWindow>())
                GetWindow<InstallerMainWindow>(true, "Runi Engine Installer");
            else
                FocusWindowIfItsOpen<InstallerMainWindow>();
        }

        const string music = "https://youtu.be/G0WZTlLJlHg";
        async UniTaskVoid MusicPlay()
        {
            VideoDestroy();
            Repaint();

            AudioReset();

            videoPlayer = Instantiate(ResourceUtility.emptyTransform.gameObject, null).AddComponent<VideoPlayer>();
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

        static void AudioReset()
        {
            AudioSettings.Reset(AudioSettings.GetConfiguration());

            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                videoPlayer.Play();
            }
        }

        static List<IInstallerScreen> installerScreens = new List<IInstallerScreen>();
        static GUIStyle? headStyle;

        static Vector2 scrollPosition = Vector2.zero;
        static int screenIndex = 0;
        static float musicVolume = 0.5f;
        static StorableClass? languageStorableClass;

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

                        GUILayout.EndScrollView();
                    }
                }
            }

            //Button
            {
                EditorTool.DrawLine(2, 0);
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.Space(6);

                {
                    {
                        EditorGUI.BeginDisabledGroup(screenIndex <= 0);

                        if (GUILayout.Button("<"))
                        {
                            screenIndex--;
                            scrollPosition = Vector2.zero;
                        }

                        EditorGUI.EndDisabledGroup();
                    }

                    GUILayout.Label($"{screenIndex + 1}/{installerScreens.Count}");

                    {
                        EditorGUI.BeginDisabledGroup(screenIndex >= installerScreens.Count - 1);

                        if (GUILayout.Button(">"))
                        {
                            screenIndex++;
                            scrollPosition = Vector2.zero;
                        }

                        EditorGUI.EndDisabledGroup();
                    }

                    if (!Kernel.isPlaying)
                    {
                        GUILayout.Space(5);

                        if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url))
                        {
                            {
                                GUILayout.Label($"{EditorTool.TryGetText("gui.volume")} {(musicVolume * 100).Floor()}/100");
                                float volume = GUILayout.HorizontalSlider(musicVolume, 0, 1, GUILayout.Width(100));

                                if (musicVolume != volume)
                                {
                                    musicVolume = volume;
                                    videoPlayer.SetDirectAudioVolume(0, musicVolume);
                                }
                            }

                            GUILayout.Space(10);

                            if (GUILayout.Button(EditorTool.TryGetText("gui.audio_reset")))
                                AudioReset();
                        }
                        else
                            GUILayout.Label(EditorTool.TryGetText("installer.music_loading"));

                        GUILayout.FlexibleSpace();

                        {
                            languageStorableClass ??= new StorableClass(typeof(EditorTool.ProjectData));
                            languageStorableClass.AutoNameLoad(Kernel.projectDataPath);

                            var languageIndex = EditorTool.ProjectData.currentLanguage switch
                            {
                                "en_us" => 0,
                                "ko_kr" => 1,
                                "ja_jp" => 2,
                                _ => 0,
                            };

                            int selectedLanguageIndex = EditorGUILayout.Popup(languageIndex, new string[] {
                                $"{EditorTool.TryGetText("language.name", "en_us")} ({EditorTool.TryGetText("language.region", "en_us")})",
                                $"{EditorTool.TryGetText("language.name", "ko_kr")} ({EditorTool.TryGetText("language.region", "ko_kr")})",
                                $"{EditorTool.TryGetText("language.name", "ja_jp")} ({EditorTool.TryGetText("language.region", "ja_jp")})"
                            }, GUILayout.Width(120));

                            if (selectedLanguageIndex != languageIndex)
                            {
                                EditorTool.ProjectData.currentLanguage = selectedLanguageIndex switch
                                {
                                    0 => "en_us",
                                    1 => "ko_kr",
                                    2 => "ja_jp",
                                    _ => "en_us",
                                };

                                languageStorableClass.AutoNameSave(Kernel.projectDataPath);
                            }
                        }
                    }
                }

                GUILayout.Space(6);
                GUILayout.EndHorizontal();
                GUILayout.Space(6);
            }
        }
    }
}
