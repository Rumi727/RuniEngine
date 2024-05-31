#nullable enable
using RuniEngine.Editor.APIBridge.UnityEditor;
using RuniEngine.Resource.Sounds;
using RuniEngine.Sounds;
using System;
using UnityEditor;

using EditorGUI = UnityEditor.EditorGUI;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioPlayer))]
    public class AudioPlayerEditor : SoundPlayerBaseEditor
    {
        static AudioFilterGUI? _audioFilterGUIInstance;
        public static AudioFilterGUI audioFilterGUIInstance => _audioFilterGUIInstance ??= AudioFilterGUI.CreateInstance();

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();

            AudioPlayer audioPlayer = (AudioPlayer)target;
            if (audioPlayer.audioSource == null || audioPlayer.audioSource.bypassEffects)
                return;
            
            if (AudioUtil.HasAudioCallback(target) && AudioUtil.GetCustomFilterChannelCount(target) > 0)
            {
                DrawLine();
                audioFilterGUIInstance.DrawAudioFilterGUI(target);
            }
        }

        protected override void NameSpaceKeyGUI()
        {
            if (target == null)
                return;

            TargetsSetValue(x => x.nameSpace, x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace), (x, y) => x.nameSpace = y, targets);
            TargetsSetValue(x => x.key, x => UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, AudioLoader.GetSoundDataKeys(x.nameSpace)), (x, y) => x.key = y, targets);
        }

        protected override void TimeSliderGUI(Action? action)
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool mixed = targets.Length > 1;
            AudioPlayer? target = (AudioPlayer?)targets[0];

            if (target == null)
                return;

            base.TimeSliderGUI(() =>
            {
                double time = target.timeSamples;
                double length = target.samples;
                double remainingTime = length - time;

                DrawTimeSliderText(target, mixed, time.ToString(), remainingTime.ToString(), length.ToString(), "", "", "", false);
            });
        }

        protected override void TimeControlGUI(Action? func, float size = 153)
        {
            if (targets == null || targets.Length <= 0)
                return;

            AudioPlayer? target = (AudioPlayer?)targets[0];
            if (target == null)
                return;

            base.TimeControlGUI(() =>
            {
                EditorGUI.BeginChangeCheck();

                long value = EditorGUILayout.LongField(target.timeSamples);

                if (EditorGUI.EndChangeCheck())
                    TargetsInvoke(x => ((AudioPlayer)x).timeSamples = value);

                func?.Invoke();
            }, size);
        }
    }
}
