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

            EditorGUILayout.LabelField(TryGetText("rect_transform.anchored_position") + ": " + TargetsToString(x => x.rectTransform.anchoredPosition));
            EditorGUILayout.LabelField(TryGetText("rect_transform.size_delta") + ": " + TargetsToString(x => x.rectTransform.sizeDelta));

            Space();

            EditorGUILayout.LabelField(TryGetText("rect_transform.offset_min") + ": " + TargetsToString(x => x.rectTransform.offsetMin));
            EditorGUILayout.LabelField(TryGetText("rect_transform.offset_max") + ": " + TargetsToString(x => x.rectTransform.offsetMax));

            Space();

            EditorGUILayout.LabelField(TryGetText("rect_transform.local_rect") + ": " + TargetsToString(x => x.rectTransform.rect));
            EditorGUILayout.LabelField(TryGetText("rect_transform.world_rect") + ": " + TargetsToString(x => x.worldCorners.rect));

            Space();

            if (GUILayout.Button(TryGetText("gui.delete"), GUILayout.ExpandWidth(false)) && targets != null)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    RectTransformTool? tool = targets[i];
                    if (tool == null)
                        continue;

                    RectTransform rectTransform = tool.rectTransform;

                    DestroyImmediate(tool);
                    Undo.DestroyObjectImmediate(rectTransform);
                }
            }
        }
    }
}
