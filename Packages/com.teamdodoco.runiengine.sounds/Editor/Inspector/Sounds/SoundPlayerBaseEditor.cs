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

            try
            {
                TargetsInvoke(x => x.LoopLock());

                string label = TryGetText("gui.loop");

                BeginLabelWidth(label);
                UseProperty(serializedObject, "_loop", label, GUILayout.Width(EditorGUIUtility.labelWidth + 17));
                EndLabelWidth();
            }
            finally
            {
                TargetsInvoke(x => x.LoopUnlock());
            }

            Space();

            try
            {
                TargetsInvoke(x => x.VolumeLock());

                string label = TryGetText("gui.volume");

                BeginLabelWidth(label);
                UseProperty(serializedObject, "_volume", label);
                EndLabelWidth();
            }
            finally
            {
                TargetsInvoke(x => x.VolumeUnlock());
            }

            Space();

            {
                EditorGUI.BeginDisabledGroup(!pitchFixedMixed && target.pitchFixed && TargetsIsEquals(x => x.tempoPitchRatio) && target.tempoPitchRatio == 0);

                bool pitchChange = false;
                try
                {
                    TargetsInvoke(x => x.PitchLock());

                    string label = TryGetText("gui.pitch");

                    EditorGUI.BeginChangeCheck();

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_pitch", label);
                    EndLabelWidth();

                    pitchChange = EditorGUI.EndChangeCheck();
                }
                finally
                {
                    TargetsInvoke(x => x.PitchUnlock());
                }

                if (pitchChange)
                {
                    TargetsInvoke(x =>
                    {
                        //프로퍼티 업데이트
                        x.pitch = x.pitch;

                        if (x.pitchFixed && x.pitchTempoRatio != 0)
                            x.tempo = x.pitch * x.tempoPitchRatio;
                    });
                }

                EditorGUI.EndDisabledGroup();
            }

            Space();

            {
                EditorGUI.BeginDisabledGroup(!pitchFixedMixed && target.pitchFixed && TargetsIsEquals(x => x.pitchTempoRatio) && target.pitchTempoRatio == 0);

                bool tempoChange = false;
                try
                {
                    TargetsInvoke(x => x.TempoLock());

                    string label = TryGetText("gui.tempo");

                    EditorGUI.BeginChangeCheck();

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_tempo", label);
                    EndLabelWidth();

                    tempoChange = EditorGUI.EndChangeCheck();
                }
                finally
                {
                    TargetsInvoke(x => x.TempoUnlock());
                }

                if (tempoChange)
                {
                    TargetsInvoke(x =>
                    {
                        //프로퍼티 업데이트
                        x.tempo = x.tempo;

                        if (x.pitchFixed && x.tempoPitchRatio != 0)
                            x.pitch = (x.tempo * x.pitchTempoRatio).Abs();
                    });
                }

                EditorGUI.EndDisabledGroup();
            }

            Space();

            {
                string label = "L";

                EditorGUI.BeginChangeCheck();

                BeginLabelWidth(label);
                UseProperty(serializedObject, "_pitchFixed", label, GUILayout.ExpandWidth(false));
                EndLabelWidth();

                if (EditorGUI.EndChangeCheck())
                {
                    TargetsInvoke(x =>
                    {
                        //프로퍼티 업데이트
                        x.pitchFixed = x.pitchFixed;
                    });
                }
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

            if (target.spatial || spatialMixed)
            {
                if (spatialMixed)
                {
                    string label = TryGetText("inspector.sound_player_base.pan_stereo");

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_panStereo", label);
                    EndLabelWidth();

                    Space();
                }

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
            else
            {
                try
                {
                    TargetsInvoke(x => x.PanStereoLock());

                    string label = TryGetText("inspector.sound_player_base.pan_stereo");

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_panStereo", label);
                    EndLabelWidth();
                }
                finally
                {
                    TargetsInvoke(x => x.PanStereoUnlock());
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

                    DrawTimeSliderText(target, mixed, time.ToTime(true, true), (length - time).ToTime(true, true), length.ToTime(true, true), realTime.ToTime(true, true), (realLength - realTime).ToTime(true, true), realLength.ToTime(true, true), true);
                    action?.Invoke();

                    //EditorGUI.EndDisabledGroup();
                }

                GUILayout.EndVertical();
            }
        }

        protected void DrawTimeSliderText(SoundPlayerBase target, bool mixed, string time, string remainingTime, string length, string realTime, string realRemainingTime, string realLength, bool realValueShow)
        {
            GUILayout.BeginHorizontal();
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(15));

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
