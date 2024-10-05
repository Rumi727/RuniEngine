#nullable enable
using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Resource.Sounds;
using System;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public abstract class SoundDataGUI
    {
        public abstract Type targetType { get; }
        public abstract string folderName { get; }

        public abstract string soundsPropertyName { get; }

        SerializedType? serializedType;
        public void DrawGUI(Rect rect, string nameSpace, string key, SoundData[] soundData, out string editedKey)
        {
            if (serializedType == null)
            {
                serializedType ??= new SerializedType(targetType, false);
                serializedType.metaData["nameSpace"] = nameSpace;
            }

            serializedType.targetObjects = soundData;

            {
                BeginLabelWidth(50);

                rect.height = GetYSize(EditorStyles.textField);

                EditorGUI.BeginDisabledGroup(serializedType.isEditingMultipleObjects);
                EditorGUI.showMixedValue = serializedType.isEditingMultipleObjects;
                {
                    key = EditorGUI.DelayedTextField(rect, TryGetText("gui.key"), key);
                    editedKey = key;

                    rect.y += rect.height + 2;
                }
                EditorGUI.showMixedValue = false;
                EditorGUI.EndDisabledGroup();

                {
                    serializedType.GetProperty(nameof(SoundData.subtitle))?.DrawGUI(rect, TryGetText("gui.subtitle"));
                    rect.y += rect.height + 2;
                }

                rect.height = GetYSize(EditorStyles.toggle);

                {
                    serializedType.GetProperty(nameof(SoundData.isBGM))?.DrawGUI(rect, "is BGM");
                    rect.y += rect.height + 2;
                }

                EndLabelWidth();
            }
            
            serializedType.GetProperty(soundsPropertyName)?.DrawGUI(rect, TryGetText("gui.sound"));
        }

        public virtual float GetHeight() => (GetYSize(EditorStyles.textField) * 2) + GetYSize(EditorStyles.toggle) + (serializedType?.GetProperty(soundsPropertyName)?.GetPropertyHeight() ?? 0) + 6;
    }
}
