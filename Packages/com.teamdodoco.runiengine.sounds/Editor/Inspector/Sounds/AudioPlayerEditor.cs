#nullable enable
using RuniEngine.Resource.Sounds;
using RuniEngine.Sounds;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioPlayer))]
    public class AudioPlayerEditor : SoundPlayerBaseEditor
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



        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();

            object[] monos = new object[] { target };
            if ((bool)audioUtilHasAudioCallbackMethod.Invoke(null, monos) && ((int)audioUtilGetCustomFilterChannelCountMethod.Invoke(null, monos)) > 0)
            {
                DrawLine();

                try
                {
                    audioFilterGUIMethod.Invoke(audioFilterGUIInstance, new object[] { target });
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException.GetType() != typeof(NullReferenceException))
                        throw;
                }
            }
        }

        protected override void NameSpaceKeyGUI()
        {
            if (target == null)
                return;

            TargetsSetValue(x => x.nameSpace, x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace), (x, y) => x.nameSpace = y, targets);
            TargetsSetValue(x => x.key, x => UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, AudioLoader.GetSoundDataKeys(x.nameSpace)), (x, y) => x.key = y, targets);
        }

        protected override void TimeSliderGUI(Action? func)
        {
            if (targets == null || targets.Length <= 0)
                return;

            AudioPlayer? target = (AudioPlayer?)targets[0];
            if (target == null)
                return;

            base.TimeSliderGUI(() =>
            {
                EditorGUI.BeginChangeCheck();
                int value = EditorGUILayout.IntField(target.timeSamples, GUILayout.Width(75));
                if (EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        AudioPlayer? target2 = (AudioPlayer?)targets[i];
                        if (target2 != null)
                            target2.timeSamples = value;
                    }
                }
                func?.Invoke();
            });
        }
    }
}
