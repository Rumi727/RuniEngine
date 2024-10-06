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

            //로드 타입
            string jsonPath = realAudioPath + ".json";

            AudioFileMetaData? fileMetaData = JsonManager.JsonRead<AudioFileMetaData>(jsonPath);
            RawAudioLoadType loadType = RawAudioLoadType.instant;
            if (fileMetaData != null)
                loadType = fileMetaData.Value.loadType;

            EditorGUI.BeginChangeCheck();
            
            {
                EditorGUI.showMixedValue = childSerializedType?.GetProperty(nameof(SoundMetaDataBase.path))?.isMixed ?? false;

                string label = TryGetText("gui.load");
                BeginLabelWidth(label);

                loadType = (RawAudioLoadType)EditorGUI.EnumPopup(position, label, loadType);
                EndLabelWidth();

                EditorGUI.showMixedValue = false;
            }

            if (!EditorGUI.EndChangeCheck() || !File.Exists(realAudioPath))
                return;

            if (loadType == RawAudioLoadType.instant)
            {
                if (!File.Exists(jsonPath))
                    return;

                File.Delete(jsonPath);
                File.Delete(jsonPath + ".meta");

                AssetDatabase.Refresh();
            }
            else
            {
                File.WriteAllText(jsonPath, JsonManager.ToJson(new AudioFileMetaData(loadType)));
                AssetDatabase.Refresh();
            }
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
            
            return base.InternalGetPropertyHeight() + 2 + (GetYSize(EditorStyles.numberField) *
#if ENABLE_RUNI_ENGINE_RHYTHMS
                    2 + 3 + (childSerializedType?.GetProperty(nameof(AudioMetaData.bpms))?.GetPropertyHeight() ?? 0) + 3
#else
                    2 + 3
#endif
                );
        }

        public override object? CreateInstance() => new AudioMetaData("", 1, 1, 0, 0, null, 0, null);
    }
}
