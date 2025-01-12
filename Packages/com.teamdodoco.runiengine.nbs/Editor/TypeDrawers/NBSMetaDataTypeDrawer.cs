#nullable enable
using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Editor.TypeDrawers;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    [CustomTypeDrawer(typeof(NBSMetaData))]
    public sealed class NBSMetaDataTypeDrawer : SoundMetaDataTypeDrawer
    {
        NBSMetaDataTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override string folderName => NBSLoader.name;
        protected override ExtensionFilter extFilter => ExtensionFilter.nbsFileFilter;

#if ENABLE_RUNI_ENGINE_RHYTHMS
        protected override void LineOtherGUI(Rect position)
        {
            if (childSerializedType == null)
                return;

            position.width *= 0.5f;
            position.width -= 1.5f;

            {
                string label = TryGetText("project_setting.nbs.bpm_multiplier");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(NBSMetaData.bpmMultiplier));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }

            position.x += position.width + 3;

            {
                string label = TryGetText("project_setting.nbs.rhythm_offset_tick");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(NBSMetaData.rhythmOffsetTick));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }
        }
#endif

#if ENABLE_RUNI_ENGINE_RHYTHMS
        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return EditorGUIUtility.singleLineHeight;

            return base.InternalGetPropertyHeight() + 2 + GetYSize(EditorStyles.numberField);
        }
#endif

        public override object? CreateInstance() =>
#if ENABLE_RUNI_ENGINE_RHYTHMS
            new NBSMetaData("", 1, 1, 1, 0, null);
#else
            new NBSMetaData("", 1, 1, null);
#endif
    }
}
