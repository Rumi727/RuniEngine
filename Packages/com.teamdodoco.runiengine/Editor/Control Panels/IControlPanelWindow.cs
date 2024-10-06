namespace RuniEngine.Editor.ControlPanels
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
