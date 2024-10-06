using RuniEngine.UI;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AutoColorAnimation))]
    public class AutoColorAnimationEditor : UIAniBaseEditor<AutoColorAnimation>
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();
            Space();

            UseProperty("_useCustomColor", TryGetText("inspector.color_animation.use_custom_color"));
            if (TargetsIsEquals(x => x.useCustomColor))
            {
                if (target.useCustomColor)
                    UseProperty("_color", TryGetText("gui.color"));
                else
                    UseProperty("_offset", TryGetText("gui.offset"));
            }
            else
            {
                UseProperty("_color", TryGetText("gui.color"));
                UseProperty("_offset", TryGetText("gui.offset"));
            }
        }
    }
}
