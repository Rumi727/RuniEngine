#nullable enable
using RuniEngine.Datas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Install
{
    public sealed class InstallerMainWindow : EditorWindow
    {
        public static InstallerMainWindow? instance;
        public static AudioSource? audioSource;

        public Texture2D? logoTexture;
        public static Stopwatch stopwatch = new Stopwatch();

        public AudioClip? musicClip;



        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            {
                installerScreens.Clear();

                IReadOnlyList<Type> types = ReflectionUtility.types;
                for (int i = 0; i < types.Count; i++)
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

            minSize = new Vector2(584, 298);
            maxSize = minSize;

            scrollPosition = Vector2.zero;
            screenIndex = 0;
            musicVolume = 0.5f;

            instance = this;

            for (int i = 0; i < installerScreens.Count; i++)
                installerScreens[i].installerMainWindow = this;

            AssemblyReloadEvents.beforeAssemblyReload += MusicDestroy;
            AssemblyReloadEvents.afterAssemblyReload += MusicPlay;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= MusicDestroy;
            AssemblyReloadEvents.afterAssemblyReload -= MusicPlay;

            MusicDestroy();
        }

        void Update()
        {
            if (audioSource != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                    audioSource.time = lastAudioTime;
                }

                lastAudioTime = audioSource.time;
            }
            else
                MusicPlay();
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

        static float lastAudioTime = 0;
        void MusicPlay()
        {
            MusicDestroy();
            Repaint();

            AudioReset();

            audioSource = Instantiate(ResourceUtility.emptyTransform.gameObject, null).AddComponent<AudioSource>();

            audioSource.name = $"{nameof(InstallerMainWindow)} Background Music";
            audioSource.gameObject.hideFlags = HideFlags.HideAndDontSave;

            audioSource.clip = musicClip;
            audioSource.volume = musicVolume;
            audioSource.loop = true;

            audioSource.Play();
            audioSource.time = lastAudioTime;

            Repaint();
        }

        static void MusicDestroy()
        {
            if (audioSource != null)
            {
                lastAudioTime = audioSource.time;
                audioSource.Stop();

                DestroyImmediate(audioSource.gameObject);
            }
        }

        static void AudioReset() => AudioSettings.Reset(AudioSettings.GetConfiguration());

        static List<IInstallerScreen> installerScreens = new List<IInstallerScreen>();
        static GUIStyle? headStyle;

        static Vector2 scrollPosition = Vector2.zero;
        static int screenIndex = 0;
        static float musicVolume = 0.5f;
        static StorableClass? languageStorableClass;

        public static void DrawGUI()
        {
            GUILayout.Space(3);
            headStyle ??= new GUIStyle(boldLabelStyle) { fontSize = 18 };

            //Screen
            {
                if (screenIndex.Clamp(0) < installerScreens.Count)
                {
                    IInstallerScreen screen = installerScreens[screenIndex];

                    if (!screen.headDisable)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(55);

                        GUILayout.BeginVertical();
                        GUILayout.Space(13);

                        EditorGUILayout.LabelField(screen.label, headStyle);

                        GUILayout.Space(13);
                        GUILayout.EndVertical();


                        GUILayout.EndHorizontal();

                        DrawLine(2, 0);
                    }

                    {
                        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                        screen.DrawGUI();

                        GUILayout.EndScrollView();
                    }
                }
            }

            DrawLogo();

            //Button
            {
                DrawLine(2, 0);

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

                    GUILayout.Space(5);

                    {
                        if (audioSource != null && audioSource.clip != null)
                        {
                            {
                                GUILayout.Label($"{TryGetText("gui.volume")} {(musicVolume * 100).Floor()}/100");
                                float volume = GUILayout.HorizontalSlider(musicVolume, 0, 1, GUILayout.Width(100));

                                if (musicVolume != volume)
                                {
                                    musicVolume = volume;
                                    audioSource.volume = musicVolume;
                                }
                            }

                            GUILayout.Space(10);

                            if (GUILayout.Button(TryGetText("gui.audio_reset")))
                                AudioReset();
                        }
                        else
                            GUILayout.Label(TryGetText("installer.music_loading"));
                    }

                    GUILayout.FlexibleSpace();

                    {
                        languageStorableClass ??= new StorableClass(typeof(ProjectData));
                        languageStorableClass.AutoNameLoad(Kernel.projectSettingPath);

                        var languageIndex = ProjectData.currentLanguage switch
                        {
                            "en_us" => 0,
                            "ko_kr" => 1,
                            "ja_jp" => 2,
                            _ => 0,
                        };

                        int selectedLanguageIndex = EditorGUILayout.Popup(languageIndex, new string[] {
                                $"{TryGetText("language.name", "en_us")} ({TryGetText("language.region", "en_us")})",
                                $"{TryGetText("language.name", "ko_kr")} ({TryGetText("language.region", "ko_kr")})",
                                $"{TryGetText("language.name", "ja_jp")} ({TryGetText("language.region", "ja_jp")})"
                            }, GUILayout.Width(120));

                        if (selectedLanguageIndex != languageIndex)
                        {
                            ProjectData.currentLanguage = selectedLanguageIndex switch
                            {
                                0 => "en_us",
                                1 => "ko_kr",
                                2 => "ja_jp",
                                _ => "en_us",
                            };

                            languageStorableClass.AutoNameSave(Kernel.projectSettingPath);
                        }
                    }
                }

                GUILayout.Space(6);
                GUILayout.EndHorizontal();
                GUILayout.Space(6);
            }
        }

        static Vector2 pos = new Vector2(114, 77);
        static float rotation = 0;
        static float size = 100;
        public static void DrawLogo()
        {
            float timer = (float)stopwatch.Elapsed.TotalSeconds;
            float lastRotation = rotation;

            if (screenIndex == 0)
            {
                pos = pos.Lerp(new Vector2(Screen.width * 0.5f - 178, Screen.height * 0.5f - 65.5f), 0.01f);
                size = size.Lerp(100, 0.01f);

                rotation = rotation.LerpAngle((timer * 64).Repeat(360), 0.01f);
            }
            else if (screenIndex >= 0 && screenIndex < installerScreens.Count && installerScreens[screenIndex].headDisable)
            {
                pos = pos.Lerp(new Vector2(7, -42), 0.03f);
                size = size.Lerp(38, 0.03f);

                rotation = rotation.LerpAngle(0, 0.02f);
            }
            else
            {
                pos = pos.Lerp(new Vector2(7, 5), 0.03f);
                size = size.Lerp(38, 0.03f);

                rotation = rotation.LerpAngle(0, 0.02f);
            }


            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

            Rect rect = new Rect(pos, new Vector2(size, size));

            Matrix4x4 matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(rotation, rect.center);

            GUI.DrawTexture(rect, instance != null ? instance.logoTexture : null);

            GUI.matrix = matrix;
            GUI.EndGroup();

            if (instance != null && lastRotation != rotation)
                instance.Repaint();
        }
    }
}
