#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;

namespace RuniEngine.Editor.TypeDrawers
{
    public abstract class NumberFieldTypeDrawer : FieldTypeDrawer
    {
        protected NumberFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        public override float GetPropertyHeight() => EditorTool.GetYSize(EditorStyles.numberField);
    }
}
