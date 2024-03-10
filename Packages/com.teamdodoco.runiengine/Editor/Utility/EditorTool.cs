#nullable enable
using UnityEditor;

namespace RuniEngine.Editor
{
    [InitializeOnLoad]
    public partial class EditorTool : UnityEditor.Editor
    {
        static EditorTool() => Selection.selectionChanged += ClearCache;

        static void ClearCache() => usePropertyAnimBoolList.Clear();
    }
}
