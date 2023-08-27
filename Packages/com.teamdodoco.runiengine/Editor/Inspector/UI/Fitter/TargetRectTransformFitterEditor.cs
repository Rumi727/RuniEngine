#nullable enable
using RuniEngine.UI.Fitter;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TargetRectTransformFitter))]
    public class TargetRectTransformFitterEditor : CustomInspectorBase<TargetRectTransformFitter>
    {
        public override void OnInspectorGUI() => UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));
    }
}
