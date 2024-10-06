using RuniEngine.Editor.Inspector.UI;
using RuniEngine.UI;
using UnityEditor;

namespace RuniEngine.Editor.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorReadability))]
    public class ColorReadabilityEditor : UIAniBaseEditor<ColorReadability>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Space();

            UseProperty("_targetCanvasRenderer", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.canvas_renderer")));
            UseProperty("_targetGraphic", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.graphic")));

            Space();

            UseProperty("_whiteColor", TryGetText("gui.color.white"));
            UseProperty("_blackColor", TryGetText("gui.color.black"));
        }
    }
}
