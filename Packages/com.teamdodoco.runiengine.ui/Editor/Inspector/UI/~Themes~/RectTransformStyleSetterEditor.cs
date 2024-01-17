#nullable enable
using RuniEngine.UI.Themes;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Themes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectTransformStyleSetter))]
    public class RectTransformStyleSetterEditor : StyleSetterBaseEditor<RectTransformStyleSetter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            UseProperty("_targetRectTransform", TryGetText("gui.rect_transform"));

            Space();

            base.OnInspectorGUI();
        }
    }
}
