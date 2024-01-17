#nullable enable
using RuniEngine.UI.Themes;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Themes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextureStyleSetter))]
    public class TextureStyleSetterEditor : StyleSetterBaseEditor<TextureStyleSetter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            UseProperty("_blurImage", TryGetText("inspector.texture_style.blurImage"));
            UseProperty("_image", TryGetText("inspector.texture_style.image"));

            Space();

            base.OnInspectorGUI();
        }
    }
}
