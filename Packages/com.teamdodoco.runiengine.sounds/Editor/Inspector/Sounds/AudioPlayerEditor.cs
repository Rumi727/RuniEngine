#nullable enable
using RuniEngine.Resource.Sounds;
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
