#nullable enable
using RuniEngine.Sounds;
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioPlayer))]
    public class AudioPlayerEditor : SoundPlayerBaseEditor
    {
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
                double value = EditorGUILayout.DoubleField(target.currentSampleIndex, GUILayout.Width(75));

                if (EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        AudioPlayer? target2 = (AudioPlayer?)targets[i];
                        if (target2 != null)
                            target2.currentSampleIndex = value;
                    }
                }

                func?.Invoke();
            });
        }
    }
}
