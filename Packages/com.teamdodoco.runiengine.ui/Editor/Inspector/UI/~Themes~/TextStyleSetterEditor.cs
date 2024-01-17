#nullable enable
using RuniEngine.UI.Themes;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Themes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextStyleSetter))]
    public class TextStyleSetterEditor : StyleSetterBaseEditor<TextStyleSetter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            UseProperty("_text", TryGetText("gui.text"));

            Space();

            base.OnInspectorGUI();
        }
    }
}
