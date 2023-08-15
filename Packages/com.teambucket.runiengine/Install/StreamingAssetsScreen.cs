#nullable enable
namespace RuniEngine.Install
{
    public sealed class StreamingAssetsScreen : IInstallerScreen
    {
        public string label { get; } = "기본 리소스 복사";
        public bool headDisable { get; } = false;
        public int sort { get; } = 3;

        public void DrawGUI()
        {
            
        }
    }
}
