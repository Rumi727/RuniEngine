#nullable enable
using RuniEngine.Editor.SerializedTypes;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    public abstract class NumberFieldTypeDrawer : FieldTypeDrawer
    {
        protected NumberFieldTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected MinAttribute? minAttribute => property.GetCustomAttribute<MinAttribute>();
        protected RangeAttribute? rangeAttribute => property.GetCustomAttribute<RangeAttribute>();

        public override float GetPropertyHeight() => EditorTool.GetYSize(EditorStyles.numberField);
    }
}
