using RuniEngine.Splashs.UI;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.Splashs.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SplashScreenProgressBar))]
    public class SplashScreenProgressBarEditor : CustomInspectorBase<SplashScreenProgressBar>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            UseProperty("_progress", TryGetText("gui.progress"));
            UseProperty("_right", TryGetText("gui.right"));

            Space();

            UseProperty("_allowNoResponseAni", TryGetText("inspector.progress.allowNoResponseAni"));

            Space();

            UseProperty("_fill", "Fill");
        }
    }
}
