#nullable enable
using Cysharp.Threading.Tasks;

namespace RuniEngine.Resource
{
    public interface IResourceElement
    {
        ResourcePack? resourcePack { get; set; }
        string name { get; }

        UniTask Load();
        UniTask Unload();
    }
}
