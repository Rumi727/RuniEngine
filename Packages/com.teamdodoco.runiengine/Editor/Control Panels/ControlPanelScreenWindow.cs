#nullable enable
using RuniEngine.Screens;
using UnityEditor;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public class ControlPanelScreenWindow : IControlPanelWindow
    {
        public string label { get; } = "gui.screen";
        public int sort { get; } = 600;

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
