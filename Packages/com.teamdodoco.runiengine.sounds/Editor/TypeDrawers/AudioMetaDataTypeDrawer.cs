#nullable enable
using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Editor.TypeDrawers;
using RuniEngine.Jsons;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System.IO;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    [CustomTypeDrawer(typeof(AudioMetaData))]
    public sealed class AudioMetaDataTypeDrawer : SoundMetaDataTypeDrawer
    {
        AudioMetaDataTypeDrawer(SerializedTypeProperty property) : base(property) { }

        protected override string folderName => AudioLoader.name;
        protected override ExtensionFilter extFilter => ExtensionFilter.musicFileFilter;

        protected override void Line2GUI(Rect position)
        {
            position.width -= 143;

            base.Line2GUI(position);

            position.x += position.width + 3;
            position.width = 140;

            position.height = GetYSize(EditorStyles.miniPullDown);

            RawAudioLoadType? loadType = null;
            bool loadTypeMixed = false;

            for (int i = 0; i < selectedMetaDatas.Length; i++)
            {
                AudioFileMetaData fileMetaData = selectedMetaDatas[i];
                if (loadType != null && loadType != fileMetaData.loadType)
                {
                    loadTypeMixed = true;
                    break;
                }

                loadType = fileMetaData.loadType;
            }

            loadType ??= RawAudioLoadType.instant;
            EditorGUI.BeginChangeCheck();

            {
                EditorGUI.showMixedValue = loadTypeMixed;

                string label = TryGetText("gui.load");
                BeginLabelWidth(label);

                loadType = (RawAudioLoadType)EditorGUI.EnumPopup(position, label, loadType);
                EndLabelWidth();

                EditorGUI.showMixedValue = false;
            }

            if (!EditorGUI.EndChangeCheck())
                return;

            for (int i = 0; i < selectedAudioPaths.Length; i++)
            {
                string? audioPath = selectedAudioPaths[i];

                AudioFileMetaData metaData = selectedMetaDatas[i];
                metaData.loadType = (RawAudioLoadType)loadType;

                if (loadType != RawAudioLoadType.instant)
                    File.WriteAllText(audioPath + ".json", JsonManager.ToJson(metaData));
                else
                {
                    File.Delete(audioPath + ".json");
                    File.Delete(audioPath + ".json.meta");
                }
            }

            AssetDatabase.Refresh();
        }

        protected override void LineOtherGUI(Rect position)
        {
            if (childSerializedType == null)
                return;

            float orgX = position.x;
            float orgWidth = position.width;

            position.width *= 0.5f;
            position.width -= 1.5f;

            {
                string label = TryGetText("project_setting.audio.loop_start_index");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(AudioMetaData.loopStartIndex));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }

            position.x += position.width + 3;

            {
                string label = TryGetText("project_setting.audio.loop_offset_index");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(AudioMetaData.loopOffsetIndex));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }

            position.x = orgX;
            position.width = orgWidth;

            position.y += position.height + 3;

#if ENABLE_RUNI_ENGINE_RHYTHMS
            {
                string label = TryGetText("inspector.rhythmable.bpm");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(AudioMetaData.bpms));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }

            position.x = orgX;
            position.width = orgWidth;

            position.y += position.height + 3;

            {
                string label = TryGetText("project_setting.audio.rhythm_offset_index");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(AudioMetaData.rhythmOffsetIndex));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }
#endif
        }

        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return EditorGUIUtility.singleLineHeight;

            return base.InternalGetPropertyHeight() + 2 +
#if ENABLE_RUNI_ENGINE_RHYTHMS
                    (GetYSize(EditorStyles.numberField) * 2 + 3 + (childSerializedType?.GetProperty(nameof(AudioMetaData.bpms))?.GetPropertyHeight() ?? 0) + 3);
#else
                    GetYSize(EditorStyles.numberField);
#endif
        }

        public override object? CreateInstance() =>
#if ENABLE_RUNI_ENGINE_RHYTHMS
            new AudioMetaData("", 1, 1, 0, 0, null, 0, null);
#else
            new AudioMetaData("", 1, 1, 0, 0, null);
#endif
    }
}
