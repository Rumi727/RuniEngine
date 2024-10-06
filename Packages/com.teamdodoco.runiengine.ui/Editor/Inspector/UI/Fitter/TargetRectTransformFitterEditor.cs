using RuniEngine.UI.Fitter;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TargetRectTransformFitter))]
    public class TargetRectTransformFitterEditor : FitterAniBaseEditor<TargetRectTransformFitter>
    {
        public override void OnInspectorGUI()
        {
            if (targets == null || targets.Length <= 0 || target == null)
                return;

            base.OnInspectorGUI();
            Space();

            UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));

            if (targets.Length <= 1 && target.rectTransform.lossyScale != Vector3.one && target.rectTransform.rotation != Quaternion.identity)
                EditorGUILayout.HelpBox(TryGetText("inspector.target_rect_transform_fitter_editor.warning"), MessageType.Warning);
        }
    }
}
