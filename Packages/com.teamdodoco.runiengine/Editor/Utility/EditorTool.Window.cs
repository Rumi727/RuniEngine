using RuniEngine.Editor.APIBridge.UnityEditor;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void RepaintCurrentWindow() => GUIView.current.Repaint();
    }
}
