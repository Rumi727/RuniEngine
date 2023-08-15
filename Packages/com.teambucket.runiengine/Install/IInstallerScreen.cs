#nullable enable
namespace RuniEngine.Install
{
    public interface IInstallerScreen
    {
        string label { get; }
        bool headDisable { get; }

        int sort { get; }

        void DrawGUI();
    }
}
