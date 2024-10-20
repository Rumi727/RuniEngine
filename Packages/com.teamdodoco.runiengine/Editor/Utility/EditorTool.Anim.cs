using System;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public bool FadeGroup(AnimBool animBool, Action action) => FadeGroup(animBool, Repaint, action);
        public bool FadeGroup(ref AnimBool? animBool, bool target, Action action) => FadeGroup(ref animBool, Repaint, target, action);

        public static bool FadeGroup(AnimBool animBool, Action repaintAction, Action action)
        {
            if (EditorGUILayout.BeginFadeGroup(animBool.faded))
            {
                try
                {
                    if (animBool.isAnimating)
                        repaintAction.Invoke();

                    action.Invoke();
                }
                finally
                {
                    EditorGUILayout.EndFadeGroup();
                    Space(-2f.Lerp(0, animBool.faded).RoundToInt());
                }

                return true;
            }

            EditorGUILayout.EndFadeGroup();

            return false;
        }

        public static bool FadeGroup(ref AnimBool? animBool, Action repaintAction, bool target, Action action)
        {
            animBool ??= new AnimBool(target);
            animBool.target = target;

            return FadeGroup(animBool, repaintAction, action);
        }
    }
}
