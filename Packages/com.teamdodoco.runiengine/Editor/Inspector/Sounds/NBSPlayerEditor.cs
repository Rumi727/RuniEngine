#nullable enable
using RuniEngine.Resource.Sounds;
using RuniEngine.Sounds;
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NBSPlayer))]
    public class NBSPlayerEditor : SoundPlayerBaseEditor
    {
        protected override void NameSpaceKeyGUI()
        {
            if (target == null)
                return;

            TargetsSetValue(x => x.nameSpace, x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace), (x, y) => x.nameSpace = y, targets);
            TargetsSetValue(x => x.key, x => UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, NBSLoader.GetSoundDataKeys(x.nameSpace)), (x, y) => x.key = y, targets);
        }

        protected override void TimeSliderGUI(Action? func)
        {
            if (targets == null || targets.Length <= 0)
                return;

            NBSPlayer? target = (NBSPlayer?)targets[0];
            if (target == null)
                return;

            base.TimeSliderGUI(() =>
            {
                {
                    EditorGUI.BeginChangeCheck();
                    double value = EditorGUILayout.DoubleField(target.tick, GUILayout.Width(50));

                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            NBSPlayer? target2 = (NBSPlayer?)targets[i];
                            if (target2 != null)
                                target2.tick = value;
                        }
                    }
                }

                {
                    EditorGUI.BeginChangeCheck();
                    int value = EditorGUILayout.IntField(target.index, GUILayout.Width(50));

                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            NBSPlayer? target2 = (NBSPlayer?)targets[i];
                            if (target2 != null)
                                target2.index = value;
                        }
                    }
                }

                func?.Invoke();
            });
        }
    }
}
