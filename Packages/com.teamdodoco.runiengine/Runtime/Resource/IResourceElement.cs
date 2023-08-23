#nullable enable
using Cysharp.Threading.Tasks;

namespace RuniEngine.Resource
{
    public interface IResourceElement
    {
        string name { get; }

        void Clear();

        UniTask Refresh(string nameSpacePath, string nameSpace);

        void Apply();
    }
}
