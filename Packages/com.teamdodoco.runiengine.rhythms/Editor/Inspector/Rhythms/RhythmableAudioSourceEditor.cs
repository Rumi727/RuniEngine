#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Rhythms;
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RhythmableAudioSource))]
    public sealed class RhythmableAudioSourceEditor : CustomInspectorBase<RhythmableAudioSource>
    {
        [SerializeField] AudioClip? hitsound;

        float y = 0;
        int lastBeat = 0;
        public override void OnInspectorGUI()
        {
            UseProperty("_rhythmOffset", TryGetText("inspector.rhythmable.rhythmOffset"));

            Space();

            {
                string propertyName = "_bpm";

                EditorGUI.BeginChangeCheck();

                try
                {
                    SerializedProperty? tps = serializedObject.FindProperty(propertyName);

                    tps.Next(true);
                    EditorGUILayout.PropertyField(tps);

                    tps.Next(true);
                    EditorGUILayout.PropertyField(tps);
                }
                catch
                {
                    GUILayout.Label(TryGetText("inspector.property_none").Replace("{name}", propertyName));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    TargetsInvoke(x => x.bpm = x.bpm);

                    lastBeat = 0;
                }
            }

            if (target == null || targets == null || targets.Length > 1)
                return;

            DrawLine();

            {
                Rect backgroundRect = EditorGUILayout.BeginVertical();

                double time = target.time - target.rhythmOffset;
                double bpm = target.bpm.bpm;
                double currentBeat = time * (bpm / 60d);
                double timeSignatures = currentBeat.Repeat(target.bpm.timeSignatures);

                DrawText("gui.beat", currentBeat.ToString());
                DrawText("gui.time", $"{time.ToTimeString()} ({time})");
                DrawText("gui.time_signatures", timeSignatures.ToString());

                void DrawText(string label, string content)
                {
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.y -= y;

                    GUI.Label(rect, $"{TryGetText(label)} : {content}");
                }

                if (target.audioSource != null && (target.audioSource.isPlaying || y != 0))
                {
                    y = y.Lerp(0, Kernel.unscaledDeltaTime * ((float)bpm.Clamp(150) / 60f));
                    if (y < 0.5f)
                        y = 0;

                    bool pass = false;
                    if (target.audioSource != null)
                    {
                        if (target.audioSource.pitch >= 0)
                            pass = currentBeat.FloorToInt() > lastBeat;
                        else
                            pass = currentBeat.FloorToInt() < lastBeat;

                        pass |= currentBeat.FloorToInt().Distance(lastBeat) >= 0.1f;
                    }

                    if (Event.current.type == EventType.Repaint && pass)
                    {
                        bool firstBeat = (int)timeSignatures == 0;

                        if (firstBeat)
                            y = 8;
                        else
                            y = 4;

                        if (backgroundRect.Contains(Event.current.mousePosition))
                        {
                            if (firstBeat)
                                PlayClip(hitsound, 0.3f, 1.25f).Forget();

                            PlayClip(hitsound, 0.3f).Forget();
                        }

                        lastBeat = currentBeat.FloorToInt();
                    }

                    Repaint();
                }
                else
                    y = 0;

                EditorGUILayout.EndVertical();
            }

            DrawLine();

            {
                GUILayout.BeginHorizontal();

                TimeSliderGUI(() =>
                {
                    bool mixed = targets.Length > 1;

                    double time = target.audioSource != null ? target.audioSource.timeSamples : 0;
                    double length = target.audioSource != null && target.audioSource.clip != null ? target.audioSource.clip.samples : 0;
                    double remainingTime = length - time;

                    DrawTimeSliderText(target, mixed, "i:", time.ToString(), remainingTime.ToString(), length.ToString(), "", "", "", false);
                });
                Space();
                TimeControlGUI(() =>
                {
                    EditorGUI.BeginChangeCheck();

                    int value = EditorGUILayout.IntField(target.audioSource != null ? target.audioSource.timeSamples : 0);

                    if (EditorGUI.EndChangeCheck())
                        TargetsInvoke(x => { if (x.audioSource != null) x.audioSource.timeSamples = value; });
                });

                GUILayout.EndHorizontal();
            }
        }

        static async UniTaskVoid PlayClip(AudioClip? clip, float volume = 1, float pitch = 1)
        {
            if (clip == null)
                return;

            GameObject gameObject = new GameObject(nameof(RhythmableAudioSourceEditor) + ".Hitsound")
            {
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy
            };

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            audioSource.Play();

            await UniTask.Delay((clip.length * 1000).CeilToInt(), true);

            DestroyImmediate(gameObject);
        }

        void TimeSliderGUI(Action? action)
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool mixed = targets.Length > 1;
            RhythmableAudioSource? target = targets[0];

            if (target == null)
                return;

            double time = target.time;
            double realTime = time / (target.audioSource != null ? target.audioSource.pitch.Abs() : 1);

            double length = target.audioSource != null && target.audioSource.clip != null ? target.audioSource.clip.length : 0;
            double realLength = length / (target.audioSource != null ? target.audioSource.pitch.Abs() : 1);

            {
                GUILayout.BeginVertical();

                Space(0);

                //슬라이더
                {
                    EditorGUI.showMixedValue = mixed;
                    EditorGUI.BeginDisabledGroup((target.audioSource == null || !target.audioSource.isPlaying) && TargetsIsEquals(x => x.audioSource != null && x.audioSource.isPlaying, targets));

                    {
                        EditorGUI.BeginChangeCheck();

                        Rect rect = EditorGUILayout.GetControlRect();
                        float value = GUI.HorizontalSlider(rect, (float)target.time, 0, target.audioSource != null && target.audioSource.clip != null ? (float)target.audioSource.clip.length : 0);

                        if (EditorGUI.EndChangeCheck())
                            TargetsInvoke(x => x.time = value);
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.showMixedValue = false;
                }

                Space(-5);

                //텍스트
                {
                    //EditorGUI.BeginDisabledGroup(!Kernel.isPlaying);

                    DrawTimeSliderText(target, mixed, "s:", time.ToTimeString(), (length - time).ToTimeString(), length.ToTimeString(), realTime.ToTimeString(), (realLength - realTime).ToTimeString(), realLength.ToTimeString(), true);
                    action?.Invoke();

                    //EditorGUI.EndDisabledGroup();
                }

                GUILayout.EndVertical();
            }
        }

        void DrawTimeSliderText(RhythmableAudioSource target, bool mixed, string label, string time, string remainingTime, string length, string realTime, string realRemainingTime, string realLength, bool realValueShow)
        {
            GUILayout.BeginHorizontal();
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(15));

            {
                Rect labelRect = rect;

                labelRect.x -= 19;
                labelRect.width = 19;

                BeginFontSize(11, richLabelStyle);
                BeginAlignment(TextAnchor.MiddleRight, richLabelStyle);

                GUI.Label(labelRect, label, richLabelStyle);

                EndAlignment(richLabelStyle);
                EndFontSize(richLabelStyle);

            }

            rect.x -= 2;
            rect.width += 4;

            BeginFontSize(11, richLabelStyle);
            BeginAlignment(TextAnchor.UpperLeft, richLabelStyle);

            if (target.audioSource != null && !mixed)
            {
                if (target.audioSource.pitch.Abs() == 1 || !realValueShow)
                    GUI.Label(rect, RichNumberMSpace(time, "7"), richLabelStyle);
                else
                    GUI.Label(rect, $"{RichNumberMSpace(time, "7")} ({RichNumberMSpace(realTime, "7")})", richLabelStyle);
            }
            else
                GUI.Label(rect, "--:--");

            EndAlignment(richLabelStyle);

            BeginAlignment(TextAnchor.UpperCenter, richLabelStyle);

            if (target.audioSource != null && !mixed)
            {
                if (target.audioSource.pitch.Abs() == 1 || !realValueShow)
                    GUI.Label(rect, RichNumberMSpace(remainingTime, "7"), richLabelStyle);
                else
                    GUI.Label(rect, $"{RichNumberMSpace(remainingTime, "7")} ({RichNumberMSpace(realRemainingTime, "7")})", richLabelStyle);
            }
            else
                GUI.Label(rect, "--:--");

            EndAlignment(richLabelStyle);

            BeginAlignment(TextAnchor.UpperRight, richLabelStyle);

            if (target.audioSource != null && !mixed)
            {
                if (target.audioSource.pitch.Abs() == 1 || !realValueShow)
                    GUI.Label(rect, RichNumberMSpace(length, "7"), richLabelStyle);
                else
                    GUI.Label(rect, $"{RichNumberMSpace(length, "7")} ({RichNumberMSpace(realLength, "7")})", richLabelStyle);
            }
            else
                GUI.Label(rect, "--:--", richLabelStyle);

            EndAlignment(richLabelStyle);
            EndFontSize(richLabelStyle);

            GUILayout.EndHorizontal();
        }

        void TimeControlGUI(Action? action, float size = 153)
        {
            if (targets == null || targets.Length <= 0)
                return;

            float buttonSize = (size - 9) / 3f;

            //bool disable = !Kernel.isPlaying;
            bool mixed = targets.Length > 1;
            RhythmableAudioSource? target = targets[0];

            if (target == null)
                return;

            {
                GUILayout.BeginVertical(GUILayout.Width(size));

                //시간
                {
                    GUILayout.BeginHorizontal();
                    EditorGUI.showMixedValue = mixed;

                    {
                        EditorGUI.BeginChangeCheck();

                        double value = EditorGUILayout.DoubleField(target.time);

                        if (EditorGUI.EndChangeCheck())
                            TargetsInvoke(x => x.time = value);
                    }

                    action?.Invoke();

                    EditorGUI.showMixedValue = false;
                    GUILayout.EndHorizontal();
                }

                {
                    GUILayout.BeginHorizontal();

#if UNITY_6000_0_OR_NEWER
                    //재생
                    if (target.audioSource != null && target.audioSource.isPlaying)
                        DrawButton("▶↻", TryGetText("gui.restart"), x => { if (x.audioSource != null) x.audioSource.Play(); });
                    else
                        DrawButton("▶", TryGetText("gui.play"), x => { if (x.audioSource != null) x.audioSource.Play(); });

                    //일시 정지
                    if (target.audioSource != null && target.audioSource.isPlaying && TargetsIsEquals(x => x.audioSource != null ? x.audioSource.isPlaying : false, targets))
                        DrawButton("▮▮", TryGetText("gui.pause"), x => { if (x.audioSource != null) x.audioSource.Pause(); });
                    else
                        DrawButton("▶▮", TryGetText("gui.unpause"), x => { if (x.audioSource != null) x.audioSource.UnPause(); });
#else
                    //재생
                    if (target.audioSource != null && target.audioSource.isPlaying)
                        DrawButton("►↻", TryGetText("gui.restart"), x => { if (x.audioSource != null) x.audioSource.Play(); });
                    else
                        DrawButton("►", TryGetText("gui.play"), x => { if (x.audioSource != null) x.audioSource.Play(); });

                    //일시 정지
                    if (target.audioSource != null && target.audioSource.isPlaying && TargetsIsEquals(x => x.audioSource != null ? x.audioSource.isPlaying : false, targets))
                        DrawButton("▮▮", TryGetText("gui.pause"), x => { if (x.audioSource != null) x.audioSource.Pause(); });
                    else
                        DrawButton("►▮", TryGetText("gui.unpause"), x => { if (x.audioSource != null) x.audioSource.UnPause(); });
#endif

                    //정지
                    DrawButton("■", TryGetText("gui.stop"), x => { if (x.audioSource != null) x.audioSource.Stop(); });

                    GUILayout.EndHorizontal();

                    void DrawButton(string buttonText, string text, Action<RhythmableAudioSource> action)
                    {
                        GUILayout.BeginVertical();
                        //EditorGUI.BeginDisabledGroup(disable);

                        Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(buttonSize), GUILayout.Height(21));
                        if (GUI.Button(buttonRect, buttonText))
                            TargetsInvoke(action);

                        //EditorGUI.EndDisabledGroup();

                        BeginFontSize(11);
                        BeginAlignment(TextAnchor.UpperCenter, labelStyle);

                        Rect textRect = EditorGUILayout.GetControlRect(GUILayout.Width(buttonSize), GUILayout.Height(5));

                        textRect.x -= 100;
                        textRect.width += 200;

                        textRect.y -= 2;
                        textRect.height = 15;

                        if (buttonRect.Contains(Event.current.mousePosition))
                        {
                            GUI.Label(textRect, text);
                        }

                        EndAlignment(labelStyle);
                        EndFontSize();

                        GUILayout.EndVertical();
                    }
                }

                GUILayout.EndVertical();
            }
        }
    }
}
