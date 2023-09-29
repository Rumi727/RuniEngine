#nullable enable
using Cysharp.Threading.Tasks;

namespace RuniEngine.Resource
{
    public interface IResourceElement
    {
        string name { get; }
        UniTask Load();
    }
}
