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

            VolumePitchTempoGUI();
            SpatialGUI();

            DrawLine();

            TimeTextGUI();
            TimeSliderGUI(null);

            Space();

            TimeControlGUI();

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
                        if (x.pitchFixed)
                            x.tempo = x.pitch * x.tempo.Sign();
                    });
                }
            }

            Space();

            {
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
                        if (x.pitchFixed)
                            x.pitch = x.tempo.Abs();
                    });
                }
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
                        if (x.pitchFixed)
                            x.pitch = x.tempo.Abs();
                    });
                }
            }



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

                BeginLabelWidth(label);
                UseProperty(serializedObject, "_spatial", label, GUILayout.Width(EditorGUIUtility.labelWidth + 17));
                EndLabelWidth();
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
                    
                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_minDistance", label);
                    EndLabelWidth();
                }

                Space();

                {
                    string label = TryGetText("gui.max_distance");

                    BeginLabelWidth(label);
                    UseProperty(serializedObject, "_maxDistance", label);
                    EndLabelWidth();
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

        protected virtual void TimeTextGUI()
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
                EditorGUI.BeginDisabledGroup(!Kernel.isPlaying);
                GUILayout.BeginHorizontal();
                
                if (!mixed)
                {
                    if (target.tempo.Abs() == 1)
                        GUILayout.Label(time.ToTime(true, true));
                    else
                        GUILayout.Label($"{time.ToTime(true, true)} ({realTime.ToTime(true, true)})");
                }
                else
                    GUILayout.Label("--:--");

                GUILayout.FlexibleSpace();

                if (!mixed)
                {
                    if (target.tempo.Abs() == 1)
                        GUILayout.Label((length - time).ToTime(true, true));
                    else
                        GUILayout.Label($"{(length - time).ToTime(true, true)} ({(realLength - realTime).ToTime(true, true)})");
                }
                else
                    GUILayout.Label("--:--");

                GUILayout.FlexibleSpace();

                if (!mixed)
                {
                    if (target.tempo.Abs() == 1)
                        GUILayout.Label(length.ToTime(true, true));
                    else
                        GUILayout.Label($"{length.ToTime(true, true)} ({realLength.ToTime(true, true)})");
                }
                else
                    GUILayout.Label("--:--");

                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
        }

        protected virtual void TimeSliderGUI(Action? func)
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool mixed = targets.Length > 1;
            SoundPlayerBase? target = targets[0];

            if (target == null)
                return;

            GUILayout.BeginHorizontal();

            EditorGUI.showMixedValue = mixed;
            EditorGUI.BeginDisabledGroup(!Kernel.isPlaying || (!target.isPlaying && TargetsIsEquals(x => x.isPlaying, targets)));

            BeginFieldWidth(60);
            EditorGUI.BeginChangeCheck();
            float value = EditorGUILayout.Slider((float)target.time, 0, (float)target.length);
            EndFieldWidth();

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    SoundPlayerBase? target2 = targets[i];
                    if (target2 != null)
                        target2.time = value;
                }
            }

            func?.Invoke();

            EditorGUI.EndDisabledGroup();
            EditorGUI.showMixedValue = false;

            GUILayout.EndHorizontal();
        }

        protected virtual void TimeControlGUI()
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool disable = !Kernel.isPlaying;
            SoundPlayerBase? target = targets[0];

            if (target == null)
                return;

            GUILayout.BeginHorizontal();

            //재생
            {
                EditorGUI.BeginDisabledGroup(disable || (!target.isActiveAndEnabled && TargetsIsEquals(x => x.isActiveAndEnabled, targets)));

                if (GUILayout.Button(TryGetText("gui.play")))
                    TargetsInvoke(x => x.Play());

                EditorGUI.EndDisabledGroup();
            }

            Space();

            //일시 정지
            {
                if (!target.isPaused || !TargetsIsEquals(x => x.isPaused, targets))
                {
                    if (GUILayout.Button(TryGetText("gui.pause")))
                        TargetsInvoke(x => x.isPaused = true);
                }
                else
                {
                    if (GUILayout.Button(TryGetText("gui.unpause")))
                        TargetsInvoke(x => x.isPaused = false);
                }
            }

            Space();

            //정지
            {
                EditorGUI.BeginDisabledGroup(disable || (!target.isPlaying && TargetsIsEquals(x => x.isPlaying, targets)));

                if (GUILayout.Button(TryGetText("gui.stop")))
                    TargetsInvoke(x => x.Stop());

                EditorGUI.EndDisabledGroup();
            }

            Space();

            //새로고침
            {
                EditorGUI.BeginDisabledGroup(disable);

                if (GUILayout.Button(TryGetText("gui.refresh")))
                    TargetsInvoke(x => x.Refresh());

                EditorGUI.EndDisabledGroup();
            }

            GUILayout.EndHorizontal();
        }
    }
}
