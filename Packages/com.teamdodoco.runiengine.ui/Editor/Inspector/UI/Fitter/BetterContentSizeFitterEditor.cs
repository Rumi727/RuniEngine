using RuniEngine.UI.Fitter;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BetterContentSizeFitter))]
    public class BetterContentSizeFitterEditor : FitterBaseEditor<BetterContentSizeFitter>
    {
        public override void OnInspectorGUI()
        {
            UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));

            Space();

            UseProperty("_xSize", TryGetText("inspector.better_content_size_fitter.x_size"));
            UseProperty("_ySize", TryGetText("inspector.better_content_size_fitter.y_size"));

            Space();

            UseProperty("_offset", TryGetText("gui.offset"));
            UseProperty("_minSize", TryGetText("gui.min_size"));
            UseProperty("_maxSize", TryGetText("gui.max_size"));
        }
    }
}
