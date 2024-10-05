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

        protected override void Line2GUI()
        {
            base.Line2GUI();

            Space(5);

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

                loadType = (RawAudioLoadType)EditorGUILayout.EnumPopup(label, loadType, GUILayout.Width(140));
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

        protected override void LineOtherGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (childSerializedType == null)
                return;

            {
                string label = TryGetText("project_setting.audio.loop_start_index");
                BeginLabelWidth(label);

                childSerializedType.GetProperty(nameof(AudioMetaData.loopStartIndex))?.DrawGUILayout(label);
                EndLabelWidth();
            }

            {
                string label = TryGetText("project_setting.audio.loop_offset_index");
                BeginLabelWidth(label);

                childSerializedType.GetProperty(nameof(AudioMetaData.loopOffsetIndex))?.DrawGUILayout(label);
                EndLabelWidth();
            }

            EditorGUILayout.EndHorizontal();

#if ENABLE_RUNI_ENGINE_RHYTHMS
            {
                string label = TryGetText("project_setting.audio.rhythm_offset_index");
                BeginLabelWidth(label);

                childSerializedType.GetProperty(nameof(AudioMetaData.rhythmOffsetIndex))?.DrawGUILayout(label);
                EndLabelWidth();
            }

            {
                string label = TryGetText("inspector.rhythmable.bpm");
                BeginLabelWidth(label);

                childSerializedType.GetProperty(nameof(AudioMetaData.bpms))?.DrawGUILayout(label);
                EndLabelWidth();
            }
#endif
        }

        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return EditorGUIUtility.singleLineHeight;

            return base.InternalGetPropertyHeight() + (EditorGUIUtility.singleLineHeight *
#if ENABLE_RUNI_ENGINE_RHYTHMS
                    3 + (childSerializedType?.GetProperty(nameof(AudioMetaData.bpms))?.GetPropertyHeight() ?? 0)
#else
                    2
#endif
                );
        }

        public override object? CreateInstance() => new AudioMetaData("", 1, 1, 0, 0, null, 0, null);
    }
}
