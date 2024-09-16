#nullable enable
using RuniEngine.Resource.Sounds;
using RuniEngine.Sounds;
using System;
using UnityEditor;

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

            UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace);
            UsePropertyAndDrawStringArray(serializedObject, "_key", TryGetText("gui.key"), target.key, NBSLoader.GetNBSDataKeys(target.nameSpace));
        }

        protected override void TimeSliderGUI(Action? action)
        {
            if (targets == null || targets.Length <= 0)
                return;

            bool mixed = targets.Length > 1;
            NBSPlayer? target = (NBSPlayer?)targets[0];

            if (target == null)
                return;

            base.TimeSliderGUI(() =>
            {
                double tick = target.tick;
                double tickLength = target.tickLength;
                double remainingTick = tickLength - tick;

                int index = target.index;
                int indexLength = target.nbsFile?.nbsNotes.Length ?? 0;
                int remainingIndex = indexLength - index;

                DrawTimeSliderText(target, mixed, "t:", tick.ToString("0.00"), remainingTick.ToString("0.00"), tickLength.ToString("0.00"), (tick / target.tempo.Abs()).ToString("0.00"), (remainingTick / target.tempo.Abs()).ToString("0.00"), (tickLength / target.tempo.Abs()).ToString("0.00"), true);
                DrawTimeSliderText(target, mixed, "i:", index.ToString(), remainingIndex.ToString(), indexLength.ToString(), "", "", "", false);
            });
        }

        protected override void TimeControlGUI(Action? func, float size = 153)
        {
            if (targets == null || targets.Length <= 0)
                return;

            NBSPlayer? target = (NBSPlayer?)targets[0];
            if (target == null)
                return;

            base.TimeControlGUI(() =>
            {
                {
                    EditorGUI.BeginChangeCheck();

                    double value = EditorGUILayout.DoubleField(target.tick);

                    if (EditorGUI.EndChangeCheck())
                        TargetsInvoke(x => ((NBSPlayer)x).tick = value);
                }

                {
                    EditorGUI.BeginChangeCheck();

                    int value = EditorGUILayout.IntField(target.index);

                    if (EditorGUI.EndChangeCheck())
                        TargetsInvoke(x => ((NBSPlayer)x).index = value);
                }

                func?.Invoke();
            }, 201);
        }
    }
}
