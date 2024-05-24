#nullable enable
using RuniEngine.UI;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SplashScreenProgressBar))]
    public class SplashScreenProgressBarEditor : CustomInspectorBase<SplashScreenProgressBar>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            EditorGUI.BeginChangeCheck();

            UseProperty("_progress", TryGetText("gui.progress"));
            UseProperty("_right", TryGetText("gui.right"));

            Space();

            UseProperty("_allowNoResponseAni", TryGetText("inspector.progress.allowNoResponseAni"));

            Space();

            UseProperty("_fill", "Fill");
        }
    }
}
