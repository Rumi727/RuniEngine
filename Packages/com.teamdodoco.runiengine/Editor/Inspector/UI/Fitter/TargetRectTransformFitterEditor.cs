#nullable enable
using RuniEngine.UI.Fitter;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TargetRectTransformFitter))]
    public class TargetRectTransformFitterEditor : FitterAniBaseEditor<TargetRectTransformFitter>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Space();

            UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));
        }
    }
}
