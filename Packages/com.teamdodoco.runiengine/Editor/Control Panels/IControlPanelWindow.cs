#nullable enable
namespace RuniEngine.Editor
{
    public interface IControlPanelWindow
    {
        string label { get; }

        int sort { get; }

        bool allowUpdate { get; }
        bool allowEditorUpdate { get; }

        void OnGUI();
    }
}
