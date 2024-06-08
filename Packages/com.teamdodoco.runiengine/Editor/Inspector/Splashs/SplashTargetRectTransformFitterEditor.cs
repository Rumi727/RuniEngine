#nullable enable
using RuniEngine.Splashs.UI;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Splashs.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SplashTargetRectTransformFitter))]
    public class SplashTargetRectTransformFitterEditor : CustomInspectorBase<SplashTargetRectTransformFitter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));

            if (targets.Length <= 1 && target.rectTransform.lossyScale != Vector3.one && target.rectTransform.rotation != Quaternion.identity)
                EditorGUILayout.HelpBox(TryGetText("inspector.target_rect_transform_fitter_editor.warning"), MessageType.Warning);
        }
    }
}
