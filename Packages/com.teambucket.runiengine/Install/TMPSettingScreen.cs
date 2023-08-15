#nullable enable
using TMPro;

namespace RuniEngine.Install
{
    public sealed class TMPSettingScreen : IInstallerScreen
    {
        public string label { get; } = "TMP 리소스";
        public bool headDisable { get; } = false;

        public int sort { get; } = 2;

        TMP_PackageResourceImporter importer = new TMP_PackageResourceImporter();
        public void DrawGUI() => importer.OnGUI();
    }
}
