#nullable enable
using System;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
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
                }

                return true;
            }

            EditorGUILayout.EndFadeGroup();
            return false;
        }

        public static bool FadeGroup(AnimBool animBool, bool target, Action repaintAction, Action action)
        {
            animBool.target = target;
            return FadeGroup(animBool, repaintAction, action);
        }
    }
}
