#nullable enable
using System;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor
{
    public partial class EditorToolExtensions
    {
        public static void FadeGroup(this AnimBool animBool, Action repaintAction, Action action) => EditorTool.FadeGroup(animBool, repaintAction, action);
        public static void FadeGroup(this AnimBool animBool, bool target, Action repaintAction, Action action) => EditorTool.FadeGroup(animBool, target, repaintAction, action);
    }
}
