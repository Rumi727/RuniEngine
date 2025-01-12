#nullable enable
using RuniEngine.UI.Fitter;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ContentAspectRatioFitter))]
    public class ContentAspectRatioFitterEditor : FitterBaseEditor<ContentAspectRatioFitter>
    {
        public override void OnInspectorGUI()
        {
            UseProperty("_targetRectTransform", TryGetText("inspector.target_~").Replace("{target}", TryGetText("inspector.rect_transform")));
            UseProperty("m_AspectMode", TryGetText("inspector.content_aspect_ratio_fitter.aspect_mode"));

            if (target != null)
            {
                if (!target.IsAspectModeValid())
                    ShowNoParentWarning();
                if (!target.IsComponentValidOnObject())
                    ShowCanvasRenderModeInvalidWarning();
            }
        }

        static void ShowNoParentWarning()
        {
            string text = L10n.Tr("You cannot use this Aspect Mode because this Component's GameObject does not have a parent object.");
            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }

        static void ShowCanvasRenderModeInvalidWarning()
        {
            string text = L10n.Tr("You cannot use this Aspect Mode because this Component is attached to a Canvas with a fixed width and height.");
            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }
    }
}
