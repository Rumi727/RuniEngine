#nullable enable
using UnityEditor;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void TabSpace(int tab)
        {
            if (tab > 0)
                EditorGUILayout.Space(30 * tab);
        }
    }
}
