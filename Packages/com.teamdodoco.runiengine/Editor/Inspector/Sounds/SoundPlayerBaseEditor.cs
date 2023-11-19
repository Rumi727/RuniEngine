#nullable enable
using RuniEngine.Resource.Sounds;
using RuniEngine.Sounds;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    public class SoundPlayerBaseEditor : CustomInspectorBase<SoundPlayerBase>
    {
        static Type? _audioFilterGUIType;
        public static Type audioFilterGUIType => _audioFilterGUIType ??= editorAssembly.GetType("UnityEditor.AudioFilterGUI");


        static object? _audioFilterGUIInstance;
        public static object audioFilterGUIInstance => _audioFilterGUIInstance ??= Activator.CreateInstance(_audioFilterGUIType);


        static MethodInfo? _audioFilterGUIMethod;
        public static MethodInfo audioFilterGUIMethod => _audioFilterGUIMethod ??= audioFilterGUIType.GetMethod("DrawAudioFilterGUI");



        static Type? _audioUtilType;
        public static Type audioUtilType => _audioUtilType ??= editorAssembly.GetType("UnityEditor.AudioUtil");


        static MethodInfo? _audioUtilHasAudioCallbackMethod;
        public static MethodInfo audioUtilHasAudioCallbackMethod => _audioUtilHasAudioCallbackMethod ??= audioUtilType.GetMethod("HasAudioCallback");


        static MethodInfo? _audioUtilGetCustomFilterChannelCountMethod;
        public static MethodInfo audioUtilGetCustomFilterChannelCountMethod => _audioUtilGetCustomFilterChannelCountMethod ??= audioUtilType.GetMethod("GetCustomFilterChannelCount");



        public override void OnInspectorGUI() => DrawGUI();

        public void DrawGUI()
        {
            if (serializedObject == null || targets == null || targets.Length <= 0 || target == null)
                return;

            EditorGUI.BeginChangeCheck();

            TargetsSetValue(x => x.nameSpace, x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace), (x, y) => x.nameSpace = y, targets);
            TargetsSetValue(x => x.key, x => UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, AudioLoader.GetSoundDataKeys(x.nameSpace)), (x, y) => x.key = y, targets);

            DrawLine();

            VolumePitchTempoGUI();
            SpatialGUI();

            DrawLine();

            TimeTextGUI();
            TimeSliderGUI(null);

            Space();

            TimeControlGUI();

            object[] monos = new object[] { target };
            if ((bool)audioUtilHasAudioCallbackMethod.Invoke(null, monos) && ((int)audioUtilGetCustomFilterChannelCountMethod.Invoke(null, monos)) > 0)
            {
                DrawLine();
                VUMeterGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                    EditorUtility.SetDirty(targets[i]);
            }
        }

        protected virtual void VolumePitchTempoGUI()
        {
            if (targets == null || targets.Length <= 0)
                return;

            SoundPlayerBase? target = targets[0];
            if (target == null)
                return;

            GUILayout.BeginHorizontal();

            TargetsSetValue(x => x.loop, x =>
            {
                GUILayout.Label(TryGetText("gui.loop"), GUILayout.ExpandWidth(false));
                return EditorGUILayout.Toggle(x.loop, GUILayout.Width(17));
            }, (x, y) => x.loop = y, targets);

            Space();

            TargetsSetValue(x => x.volume, x =>
            {
                GUILayout.Label(TryGetText("gui.volume"), GUILayout.ExpandWidth(false));
                return EditorGUILayout.Slider((float)x.volume, 0, 2);
            }, (x, y) => x.volume = y, targets);

            Space();

            TargetsSetValue(x => x.pitch, x =>
            {
                GUILayout.Label(TryGetText("gui.pitch"), GUILayout.ExpandWidth(false));
                return EditorGUILayout.Slider((float)x.pitch, 0, 3);
            }, (x, y) => x.pitch = y, targets);

            Space();

            TargetsSetValue(x => x.tempo, x =>
            {
                GUILayout.Label(TryGetText("gui.tempo"), GUILayout.ExpandWidth(false));
                return EditorGUILayout.Slider((float)x.tempo, -3, 3);
            }, (x, y) => x.tempo = y, targets);

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

            bool spatialMixed = !TargetsIsEquals(x => x.spatial, targets);

            TargetsSetValue(x => x.spatial, x =>
            {
                GUILayout.Label(TryGetText("gui.spatial"), GUILayout.ExpandWidth(false));
                return EditorGUILayout.Toggle(x.spatial, GUILayout.Width(17));
            }, (x, y) => x.spatial = y, targets);

            Space();

            if (target.spatial || spatialMixed)
            {
                if (spatialMixed)
                {
                    TargetsSetValue(x => x.panStereo, x =>
                    {
                        GUILayout.Label(TryGetText("inspector.sound_player_base.pan_stereo"), GUILayout.ExpandWidth(false));
                        return EditorGUILayout.Slider(x.panStereo, -1, 1);
                    }, (x, y) => x.panStereo = y, targets);

                    Space();
                }

                GUILayout.Label(TryGetText("gui.min_distance"), GUILayout.ExpandWidth(false));
                UseProperty(serializedObject, "_minDistance", "");

                Space();

                GUILayout.Label(TryGetText("gui.max_distance"), GUILayout.ExpandWidth(false));
                UseProperty(serializedObject, "_maxDistance", "");
            }
            else
            {
                TargetsSetValue(x => x.panStereo, x =>
                {
                    GUILayout.Label(TryGetText("inspector.sound_player_base.pan_stereo"), GUILayout.ExpandWidth(false));
                    return EditorGUILayout.Slider(x.panStereo, -1, 1);
                }, (x, y) => x.panStereo = y, targets);
            }

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

            EditorGUI.BeginChangeCheck();
            float value = EditorGUILayout.Slider((float)target.time, 0, (float)target.length);

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
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        SoundPlayerBase? target2 = targets[i];
                        if (target2 != null)
                            target2.Play();
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            Space();

            //일시 정지
            {
                if (!target.isPaused || !TargetsIsEquals(x => x.isPaused, targets))
                {
                    if (GUILayout.Button(TryGetText("gui.pause")))
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            SoundPlayerBase? target2 = targets[i];
                            if (target2 != null)
                                target2.isPaused = true;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button(TryGetText("gui.unpause")))
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            SoundPlayerBase? target2 = targets[i];
                            if (target2 != null)
                                target2.isPaused = false;
                        }
                    }
                }
            }

            Space();

            //정지
            {
                EditorGUI.BeginDisabledGroup(disable || (!target.isPlaying && TargetsIsEquals(x => x.isPlaying, targets)));

                if (GUILayout.Button(TryGetText("gui.stop")))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        SoundPlayerBase? target2 = targets[i];
                        if (target2 != null)
                            target2.Stop();
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            Space();

            //새로고침
            {
                EditorGUI.BeginDisabledGroup(disable);

                if (GUILayout.Button(TryGetText("gui.refresh")))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        SoundPlayerBase? target2 = targets[i];
                        if (target2 != null)
                            target2.Refresh();
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            GUILayout.EndHorizontal();
        }

        protected virtual void VUMeterGUI()
        {
            if (target == null)
                return;

            try
            {
                audioFilterGUIMethod.Invoke(audioFilterGUIInstance, new object[] { target });
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException.GetType() != typeof(NullReferenceException))
                    throw;
            }
            
            return;
        }
    }
}