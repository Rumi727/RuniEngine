#nullable enable
namespace RuniEngine.Rendering
{
    public interface IRenderer
    {
        string nameSpace { get; set; }
        string path { get; set; }

        NameSpacePathPair pair { get; set; }

        void Refresh();
    }
}
