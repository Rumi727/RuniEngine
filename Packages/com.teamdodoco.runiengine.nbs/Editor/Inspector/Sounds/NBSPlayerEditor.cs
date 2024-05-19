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
            TargetsSetValue(x => x.key, x => UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, NBSLoader.GetNBSDataKeys(x.nameSpace)), (x, y) => x.key = y, targets);
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
                        TargetsInvoke(x => ((NBSPlayer)x).tick = value);
                }

                {
                    EditorGUI.BeginChangeCheck();

                    int value = EditorGUILayout.IntField(target.index, GUILayout.Width(50));

                    if (EditorGUI.EndChangeCheck())
                        TargetsInvoke(x => ((NBSPlayer)x).index = value);
                }

                func?.Invoke();
            });
        }
    }
}
