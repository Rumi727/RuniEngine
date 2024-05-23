#nullable enable
using Cysharp.Threading.Tasks;
using System;

namespace RuniEngine.Resource
{
    public interface IResourceElement
    {
        ResourcePack? resourcePack { get; set; }
        string name { get; }

        UniTask Load();
        UniTask Load(IProgress<float>? progress);
        UniTask Unload();
    }
}
