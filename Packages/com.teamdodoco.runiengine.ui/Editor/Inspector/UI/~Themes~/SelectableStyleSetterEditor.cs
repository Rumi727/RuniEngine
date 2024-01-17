#nullable enable
using RuniEngine.UI.Themes;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Themes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SelectableStyleSetter))]
    public class SelectableStyleSetterEditor : StyleSetterBaseEditor<SelectableStyleSetter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            UseProperty("_selectable", TryGetText("gui.selectable"));

            Space();

            base.OnInspectorGUI();
        }
    }
}
