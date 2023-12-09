#nullable enable
using UnityEditor;

namespace RuniEngine.Editor.Inspector.Rendering
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextMeshProRenderer))]
    public class TextMeshProRendererEditor : CustomInspectorBase<TextMeshProRenderer>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            EditorGUI.BeginChangeCheck();

            TargetsSetValue(x => x.nameSpace, x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace), (x, y) => x.nameSpace = y, targets);
            UseProperty("_path", TryGetText("gui.key"));

            Space();

            UseProperty("_replaces", TryGetText("gui.replace"));

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    TextMeshProRenderer? value = targets[i];
                    if (value != null)
                        value.Refresh();
                }
            }
        }
    }
}
