#nullable enable
namespace RuniEngine.Install
{
    public interface IInstallerScreen
    {
        InstallerMainWindow? installerMainWindow { get; set; }

        string label { get; }
        bool headDisable { get; }

        int sort { get; }

        void DrawGUI();
    }
}
