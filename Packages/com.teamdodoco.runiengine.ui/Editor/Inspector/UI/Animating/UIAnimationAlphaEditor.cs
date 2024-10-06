using RuniEngine.UI.Animating;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimationAlpha))]
    public class UIAnimationAlphaEditor : UIAnimationEditor<UIAnimationAlpha>
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();
            Space();

            UseProperty("_alpha", TryGetText("gui.alpha"));
        }
    }
}
