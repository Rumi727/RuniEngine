#nullable enable
using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Jsons;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public abstract class SoundMetaDataTypeDrawer : StructTypeDrawer
    {
        protected SoundMetaDataTypeDrawer(SerializedTypeProperty property) : base(property) { }

        /// <summary><see cref="Line1GUI"/> 메소드에서 마지막으로 계산한 실제 오디오 경로를 반환합니다</summary>
        //protected string realAudioPath { get; private set; } = string.Empty;

        protected abstract string folderName { get; }
        protected abstract ExtensionFilter extFilter { get; }

        public string[] selectedAudioPaths = Array.Empty<string>();
        public AudioFileMetaData[] selectedMetaDatas = Array.Empty<AudioFileMetaData>();

        protected override void InternalDrawStructGUI(Rect position, GUIContent? label)
        {
            selectedAudioPaths = GetAudioRealPaths().ToArray();
            selectedMetaDatas = selectedAudioPaths.Select(static x => JsonManager.JsonRead<AudioFileMetaData>(x, ".json", StreamingIOHandler.instance)).ToArray();

            Line1GUI(position);

            position.y += GetYSize(EditorStyles.textField) + 3;
            position.height -= GetYSize(EditorStyles.textField) + 3;

            Line2GUI(position);

            position.y += GetYSize(EditorStyles.numberField) + 2;
            position.height -= GetYSize(EditorStyles.numberField) + 2;

            LineOtherGUI(position);
        }

        protected virtual void Line1GUI(Rect position)
        {
            string nameSpace = (string)(property.metaData.GetValueOrDefault("nameSpace", null) ?? string.Empty);

            SerializedTypeProperty? pathProperty = childSerializedType?.GetProperty(nameof(SoundMetaDataBase.path));
            if (pathProperty == null)
                return;

            string label = TryGetText("gui.path");

            BeginLabelWidth(label);

            position.width -= 103;
            position.height = pathProperty.GetPropertyHeight();

            pathProperty.DrawGUI(position, label);

            EndLabelWidth();

            position.x += position.width + 3;

            string audioPath = (string?)pathProperty.GetValue() ?? string.Empty;

            EditorGUI.showMixedValue = pathProperty.isMixed;

            //Drag And Drop
            {
                string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, folderName).UniformDirectorySeparatorCharacter();
                string assetAllPathAndName = Path.Combine(assetAllPath, audioPath).UniformDirectorySeparatorCharacter();

                string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace, folderName).UniformDirectorySeparatorCharacter();
                string assetPathAndName = Path.Combine(assetPath, audioPath).UniformDirectorySeparatorCharacter();

                ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, extFilter);

                EditorGUI.BeginChangeCheck();

                position.width = 100;

                DefaultAsset? audioClip = null;
                try
                {
                    if (audioPath != "")
                    {
                        audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                        audioClip = (DefaultAsset)EditorGUI.ObjectField(position, audioClip, typeof(DefaultAsset), false);
                    }
                    else
                        audioClip = (DefaultAsset)EditorGUI.ObjectField(position, null, typeof(DefaultAsset), false);
                }
                catch (ExitGUIException) { }

                if (EditorGUI.EndChangeCheck())
                {
                    string changedAssetPathAneName = AssetDatabase.GetAssetPath(audioClip).Replace(assetPath + "/", "");
                    for (int i = 0; i < extFilter.extensions.Length; i++)
                    {
                        if (Path.GetExtension(changedAssetPathAneName) != extFilter.extensions[i])
                            continue;

                        pathProperty.SetValue(PathUtility.GetPathWithoutExtension(changedAssetPathAneName.UniformDirectorySeparatorCharacter()));
                        break;
                    }
                }
            }

            EditorGUI.showMixedValue = false;
        }

        protected virtual void Line2GUI(Rect position)
        {
            if (childSerializedType == null)
                return;

            position.width *= 0.5f;
            position.width -= 1.5f;

            {
                string label = TryGetText("gui.pitch");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(SoundMetaDataBase.pitch));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }

            position.x += position.width + 3;

            {
                string label = TryGetText("gui.tempo");
                BeginLabelWidth(label);

                SerializedTypeProperty? property = childSerializedType.GetProperty(nameof(SoundMetaDataBase.tempo));

                position.height = property?.GetPropertyHeight() ?? 0;
                property?.DrawGUI(position, label);

                EndLabelWidth();
            }
        }

        /// <summary>이 메소드는 내부 호출 시에 <see cref="EditorGUILayout.BeginHorizontal(GUILayoutOption[])"/> 메소드가 호출되지 않습니다</summary>
        protected virtual void LineOtherGUI(Rect position) { }

        protected override float InternalGetStructHeight() => GetYSize(EditorStyles.textField) + 3 + GetYSize(EditorStyles.numberField);

        IEnumerable<string> GetAudioRealPaths()
        {
            string nameSpace = (string)(property.metaData.GetValueOrDefault("nameSpace", null) ?? string.Empty);

            SerializedTypeProperty? pathProperty = childSerializedType?.GetProperty(nameof(SoundMetaDataBase.path));
            if (pathProperty == null)
                yield break;

            foreach (string? audioPath in pathProperty.GetValues().Cast<string?>())
            {
                if (audioPath == null)
                    continue;

                string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, folderName).UniformDirectorySeparatorCharacter();
                string assetAllPathAndName = Path.Combine(assetAllPath, audioPath).UniformDirectorySeparatorCharacter();

                ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, extFilter);
                yield return outPath;
            }
        }
    }
}
