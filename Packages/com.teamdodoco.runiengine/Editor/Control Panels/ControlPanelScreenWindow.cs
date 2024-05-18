#nullable enable
using RuniEngine.Screens;
using UnityEditor;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public class ControlPanelScreenWindow : IControlPanelWindow
    {
        public string label => "gui.screen";
        public int sort => 600;

        public bool allowUpdate => true;
        public bool allowEditorUpdate => false;

        static ScreenMover? screenMover;
        static ScreenCroper? screenCroper;
        public void OnGUI()
        {
            screenMover ??= new ScreenMover();
            screenCroper ??= new ScreenCroper();

            BeginWideMode(true);

            screenMover.position = EditorGUILayout.Vector3Field(TryGetText("gui.position"), screenMover.position);
            screenCroper.offset = RectOffsetField(TryGetText("gui.offset"), screenCroper.offset);

            EndWideMode();
        }
    }
}
