#nullable enable
using UnityEditor;

namespace RuniEngine.Editor.Inspector.Rendering
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextMeshProSetter))]
    public class TextMeshProSetterEditor : CustomInspectorBase<TextMeshProSetter>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            EditorGUI.BeginChangeCheck();

            UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace);
            UseProperty("_path", TryGetText("gui.key"));

            Space();

            UseProperty("_replaces", TryGetText("gui.replace"));

            if (EditorGUI.EndChangeCheck())
                TargetsInvoke(x => x.Refresh());
        }
    }
}
