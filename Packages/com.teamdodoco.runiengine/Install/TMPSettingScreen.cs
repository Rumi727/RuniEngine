#nullable enable
using RuniEngine.Editor;
using TMPro;

namespace RuniEngine.Install
{
    public sealed class TMPSettingScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label => EditorTool.TryGetText("installer.tmp_setting.label");
        public bool headDisable { get; } = false;

        public int sort { get; } = 2;

        readonly TMP_PackageResourceImporter importer = new TMP_PackageResourceImporter();
        public void DrawGUI() => importer.OnGUI();
    }
}
