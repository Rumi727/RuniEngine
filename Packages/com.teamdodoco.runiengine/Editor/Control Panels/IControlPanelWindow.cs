#nullable enable
namespace RuniEngine.Editor
{
    public interface IControlPanelWindow
    {
        string label { get; }

        int sort { get; }

        void OnGUI();
    }
}
