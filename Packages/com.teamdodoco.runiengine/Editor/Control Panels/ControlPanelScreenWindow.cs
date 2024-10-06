using RuniEngine.Editor.APIBridge.UnityEditor.UI;
using RuniEngine.Screens;
using UnityEditor;
using UnityEngine;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ControlPanels
{
    public class ControlPanelScreenWindow : IControlPanelWindow
    {
        public string label => "gui.screen";
        public int sort => 600;

        public bool allowUpdate => true;
        public bool allowEditorUpdate => true;

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

            DrawLine();

            GUILayout.Label($"{TryGetText("gui.current_position")} - {ScreenManager.screenPosition.x} {ScreenManager.screenPosition.y} {ScreenManager.screenPosition.z}");
            GUILayout.Label($"{TryGetText("gui.current_offset")} - {ScreenManager.screenArea.left} {ScreenManager.screenArea.right} {ScreenManager.screenArea.top} {ScreenManager.screenArea.bottom}");

            Rect offset = new Rect(screenMover.position.x + screenCroper.offset.left, screenMover.position.y + screenCroper.offset.bottom, screenCroper.offset.right, screenCroper.offset.top);

            Rect inner = Rect.zero;
            
            inner.x = offset.x;
            inner.y = offset.y;
            inner.width = (ScreenManager.width + offset.width - screenCroper.offset.left);
            inner.height = (ScreenManager.height + offset.height - screenCroper.offset.bottom);

            Space();

            SpriteDrawUtility.DrawSprite(Texture2D.whiteTexture, EditorGUILayout.GetControlRect(false, 200), Vector4.zero, new Rect(0, 0, ScreenManager.width, ScreenManager.height), inner, Rect.zero, new Color(0.5f, 0.5f, 0.5f, 0.75f), null);
        }
    }
}
