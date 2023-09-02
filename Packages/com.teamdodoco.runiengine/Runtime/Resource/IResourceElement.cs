#nullable enable
using Cysharp.Threading.Tasks;

namespace RuniEngine.Resource
{
    public interface IResourceElement
    {
        string name { get; }
        ResourceManager.RefreshDelegate[] refreshDelegates { get; }

        void Clear();

        void Apply();
    }
}
