#nullable enable
#if UNITY_EDITOR
using RuniEngine.Editor.Inspector;
using UnityEditor;

namespace RuniEngine.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Test))]
    public class TestEditor : CustomInspectorBase<Test>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif