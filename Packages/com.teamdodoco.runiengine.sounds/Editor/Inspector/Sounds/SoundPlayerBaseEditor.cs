#nullable enable
using RuniEngine.Sounds;
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    public abstract class SoundPlayerBaseEditor : CustomInspectorBase<SoundPlayerBase>
    {
        public override void OnInspectorGUI() => DrawGUI();

        public void DrawGUI()
        {
            if (serializedObject == null || targets == null || targets.Length <= 0 || target == null)
                return;

            EditorGUI.BeginChangeCheck();

            NameSpaceKeyGUI();

            DrawLine();

            GUILayout.BeginVertical();

            VolumePitchTempoGUI();
            SpatialGUI();

            GUILayout.EndVertical();

            DrawLine();

            {
                GUILayout.BeginHorizontal();

                TimeSliderGUI(null);
                Space();
                TimeControlGUI(null);

                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                    EditorUtility.SetDirty(targets[i]);
            }
        }

        protected abstract void NameSpaceKeyGUI();

        protected virtual void VolumePitchTempoGUI()
        {
            if (targets == null || targets.Length <= 0)
                return;

            SoundPlayerBase? target = targets[0];
            if (target == null)
                return;

            bool pitchFixedMixed = !TargetsIsEquals(x => x.pitchFixed);

            GUILayout.BeginHorizontal();
            BeginFieldWidth(40);

            {
                GUIContent label = new GUIContent(TryGetText("gui.loop"));

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_loop");
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.labelWidth + 17));

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.loop, x => EditorGUI.Toggle(rect, label, x.loop), (x, y) => x.loop = y, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();
            }

            Space();

            {
                GUIContent label = new GUIContent(TryGetText("gui.volume"));

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_volume");
                    Rect rect = EditorGUILayout.GetControlRect();

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.volume, x => EditorGUI.Slider(rect, label, x.volume, 0, 2), (x, y) => x.volume = y, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();
            }

            Space();

            {
                EditorGUI.BeginDisabledGroup(!pitchFixedMixed && target.pitchFixed && TargetsIsEquals(x => x.tempoPitchRatio) && target.tempoPitchRatio == 0);

                GUIContent label = new GUIContent(TryGetText("gui.pitch"));

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_pitch");
                    Rect rect = EditorGUILayout.GetControlRect();

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.pitch, x => EditorGUI.Slider(rect, label, (float)x.pitch, 0, 3), (x, y) =>
                    {
                        x.pitch = y;
                        if (x.pitchFixed && x.pitchTempoRatio != 0)
                            x.tempo = y * x.tempoPitchRatio;
                    }, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();

                EditorGUI.EndDisabledGroup();
            }

            Space();

            {
                EditorGUI.BeginDisabledGroup(!pitchFixedMixed && target.pitchFixed && TargetsIsEquals(x => x.pitchTempoRatio) && target.pitchTempoRatio == 0);

                GUIContent label = new GUIContent(TryGetText("gui.tempo"));

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_tempo");
                    Rect rect = EditorGUILayout.GetControlRect();

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.tempo, x => EditorGUI.Slider(rect, label, (float)x.tempo, -3, 3), (x, y) =>
                    {
                        x.tempo = y;
                        if (x.pitchFixed && x.tempoPitchRatio != 0)
                            x.pitch = (y * x.pitchTempoRatio).Abs();
                    }, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();

                EditorGUI.EndDisabledGroup();
            }

            Space();

            {
                GUIContent label = new GUIContent("L");

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_pitchFixed");
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(false));

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.pitchFixed, x => EditorGUI.Toggle(rect, label, x.pitchFixed), (x, y) => x.pitchFixed = y, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();
            }

            Space(-28);

            EndFieldWidth();
            GUILayout.EndHorizontal();
        }

        protected virtual void SpatialGUI()
        {
            if (serializedObject == null || targets == null || targets.Length <= 0)
                return;

            SoundPlayerBase? target = targets[0];
            if (target == null)
                return;

            GUILayout.BeginHorizontal();
            BeginFieldWidth(40);

            bool spatialMixed = !TargetsIsEquals(x => x.spatial, targets);

            {
                string label = TryGetText("gui.spatial");

                EditorGUI.BeginChangeCheck();

                BeginLabelWidth(label);
                UseProperty(serializedObject, "_spatial", label, GUILayout.Width(EditorGUIUtility.labelWidth + 17));
                EndLabelWidth();

                if (EditorGUI.EndChangeCheck())
                {
                    TargetsInvoke(x =>
                    {
                        //프로퍼티 업데이트
                        x.spatial = x.spatial;
                    });
                }
            }

            Space();

            {
                GUIContent label = new GUIContent(TryGetText("inspector.sound_player_base.pan_stereo"));

                BeginLabelWidth(label);

                {
                    SerializedProperty tps = serializedObject.FindProperty("_panStereo");
                    Rect rect = EditorGUILayout.GetControlRect();

                    EditorGUI.BeginProperty(rect, label, tps);
                    TargetsSetValue(x => x.panStereo, x => EditorGUI.Slider(rect, label, x.panStereo, -1, 1), (x, y) => x.panStereo = y, targets);
                    EditorGUI.EndProperty();
                }

                EndLabelWidth();
            }

            Space();

            if (target.spatial)
            {
                {
                    string label = TryGetText("gui.min_distance");

                    EditorGUI.BeginChangeCheck();
                    
                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_minDistance", label);
                    EndLabelWidth();

                    if (EditorGUI.EndChangeCheck())
                    {
                        TargetsInvoke(x =>
                        {
                            //프로퍼티 업데이트
                            x.minDistance = x.minDistance;
                        });
                    }
                }

                Space();

                {
                    string label = TryGetText("gui.max_distance");

                    EditorGUI.BeginChangeCheck();

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_maxDistance", label);
                    EndLabelWidth();

                    if (EditorGUI.EndChangeCheck())
                    {
                        TargetsInvoke(x =>
                        {
                            //프로퍼티 업데이트
                            x.maxDistance = x.maxDistance;
                        });
                    }
                }
            }

            EndFieldWidth();
            GUILayout.EndHorizontal();
        }

        protected virtual void TimeSliderGUI(Action? action)
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool mixed = targets.Length > 1;
            SoundPlayerBase? target = targets[0];

            if (target == null)
                return;

            double time = target.time;
            double realTime = target.realTime;

            double length = target.length;
            double realLength = target.realLength;

            {
                GUILayout.BeginVertical();

                Space(0);

                //슬라이더
                {
                    EditorGUI.showMixedValue = mixed;
                    EditorGUI.BeginDisabledGroup(!target.isPlaying && TargetsIsEquals(x => x.isPlaying, targets));

                    {
                        EditorGUI.BeginChangeCheck();

                        Rect rect = EditorGUILayout.GetControlRect();
                        float value = GUI.HorizontalSlider(rect, (float)target.time, 0, (float)target.length);

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

        protected void DrawTimeSliderText(SoundPlayerBase target, bool mixed, string label, string time, string remainingTime, string length, string realTime, string realRemainingTime, string realLength, bool realValueShow)
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

            if (!mixed)
            {
                if (target.tempo.Abs() == 1 || !realValueShow)
                    GUI.Label(rect, RichNumberMSpace(time, "7"), richLabelStyle);
                else
                    GUI.Label(rect, $"{RichNumberMSpace(time, "7")} ({RichNumberMSpace(realTime, "7")})", richLabelStyle);
            }
            else
                GUI.Label(rect, "--:--");

            EndAlignment(richLabelStyle);

            BeginAlignment(TextAnchor.UpperCenter, richLabelStyle);

            if (!mixed)
            {
                if (target.tempo.Abs() == 1 || !realValueShow)
                    GUI.Label(rect, RichNumberMSpace(remainingTime, "7"), richLabelStyle);
                else
                    GUI.Label(rect, $"{RichNumberMSpace(remainingTime, "7")} ({RichNumberMSpace(realRemainingTime, "7")})", richLabelStyle);
            }
            else
                GUI.Label(rect, "--:--");

            EndAlignment(richLabelStyle);

            BeginAlignment(TextAnchor.UpperRight, richLabelStyle);

            if (!mixed)
            {
                if (target.tempo.Abs() == 1 || !realValueShow)
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

        protected virtual void TimeControlGUI(Action? action, float size = 153)
        {
            if (targets == null || targets.Length <= 0)
                return;

            float buttonSize = (size - 9) / 4f;

            //bool disable = !Kernel.isPlaying;
            bool mixed = targets.Length > 1;
            SoundPlayerBase? target = targets[0];

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
                    if (target.isPlaying)
                        DrawButton("▶↻", TryGetText("gui.restart"), x => x.Play());
                    else
                        DrawButton("▶", TryGetText("gui.play"), x => x.Play());

                    //일시 정지
                    if (!target.isPaused || !TargetsIsEquals(x => x.isPaused, targets))
                        DrawButton("▮▮", TryGetText("gui.pause"), x => x.isPaused = true);
                    else
                        DrawButton("▶▮", TryGetText("gui.unpause"), x => x.isPaused = false);
#else
                    //재생
                    if (target.isPlaying)
                        DrawButton("►↻", TryGetText("gui.restart"), x => x.Play());
                    else
                        DrawButton("►", TryGetText("gui.play"), x => x.Play());

                    //일시 정지
                    if (!target.isPaused || !TargetsIsEquals(x => x.isPaused, targets))
                        DrawButton("▮▮", TryGetText("gui.pause"), x => x.isPaused = true);
                    else
                        DrawButton("►▮", TryGetText("gui.unpause"), x => x.isPaused = false);
#endif

                    //정지
                    DrawButton("■", TryGetText("gui.stop"), x => x.Stop());

                    //새로고침
                    DrawButton("↻", TryGetText("gui.refresh"), x => x.Refresh());

                    GUILayout.EndHorizontal();

                    void DrawButton(string buttonText, string text, Action<SoundPlayerBase> action)
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
