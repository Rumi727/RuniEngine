#nullable enable
using RuniEngine.UI;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectTransformTool))]
    public class RectTransformToolEditor : CustomInspectorBase<RectTransformTool>
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            EditorGUILayout.LabelField("Anchored Position: " + TargetsToString(x => x.rectTransform.anchoredPosition));
            EditorGUILayout.LabelField("Size Delta: " + TargetsToString(x => x.rectTransform.sizeDelta));

            Space();

            EditorGUILayout.LabelField("Offset Min: " + TargetsToString(x => x.rectTransform.offsetMin));
            EditorGUILayout.LabelField("Offset Max: " + TargetsToString(x => x.rectTransform.offsetMax));

            Space();

            EditorGUILayout.LabelField("Local Rect: " + TargetsToString(x => x.rectTransform.rect));
            EditorGUILayout.LabelField("World Rect: " + TargetsToString(x => x.worldCorners.rect));
        }
    }
}
