using RuniEngine.Editor.SerializedTypes;
using RuniEngine.Editor.TypeDrawers;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public abstract class SoundMetaDataTypeDrawer : ObjectTypeDrawer
    {
        protected SoundMetaDataTypeDrawer(SerializedTypeProperty property) : base(property) { }

        /// <summary><see cref="Line1GUI"/> 메소드에서 마지막으로 계산한 실제 오디오 경로를 반환합니다</summary>
        protected string realAudioPath { get; private set; } = string.Empty;

        protected abstract string folderName { get; }
        protected abstract ExtensionFilter extFilter { get; }

        Rect lastPosition;
        protected override void InternalOnGUI(Rect position, GUIContent? label)
        {
            {
                if (!property.canRead || property.DrawNullableButton(position, label, out bool isDrawed))
                    return;

                if (isDrawed)
                    position.width -= 42;
            }

            SetChild();

            if (Event.current.type == EventType.Repaint)
                lastPosition = position;

            GUILayout.BeginArea(lastPosition);

            EditorGUILayout.BeginHorizontal();
            Line1GUI();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Line2GUI();
            EditorGUILayout.EndHorizontal();

            LineOtherGUI();

            GUILayout.EndArea();
        }

        protected virtual void Line1GUI()
        {
            string nameSpace = (string)(property.metaData.GetValueOrDefault("nameSpace", null) ?? string.Empty);

            SerializedTypeProperty? pathProperty = childSerializedType?.GetProperty(nameof(SoundMetaDataBase.path));
            if (pathProperty == null)
                return;

            string label = TryGetText("gui.path");

            BeginLabelWidth(label);
            pathProperty.DrawGUILayout(label);
            EndLabelWidth();

            string audioPath = (string?)pathProperty.GetValue() ?? string.Empty;

            EditorGUI.showMixedValue = pathProperty.isMixed;

            //Drag And Drop
            {
                string assetAllPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, folderName).Replace("\\", "/");
                string assetAllPathAndName = Path.Combine(assetAllPath, audioPath).Replace("\\", "/");

                string assetPath = Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, nameSpace, folderName).Replace("\\", "/");
                string assetPathAndName = Path.Combine(assetPath, audioPath).Replace("\\", "/");

                ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, extFilter);
                realAudioPath = outPath;

                EditorGUI.BeginChangeCheck();

                DefaultAsset audioClip;
                if (audioPath != "")
                {
                    audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                    audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false, GUILayout.Width(100));
                }
                else
                    audioClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false, GUILayout.Width(100));

                if (EditorGUI.EndChangeCheck())
                {
                    string changedAssetPathAneName = AssetDatabase.GetAssetPath(audioClip).Replace(assetPath + "/", "");
                    for (int i = 0; i < extFilter.extensions.Length; i++)
                    {
                        if (Path.GetExtension(changedAssetPathAneName) != extFilter.extensions[i])
                            continue;

                        pathProperty.SetValue(PathUtility.GetPathWithExtension(changedAssetPathAneName).Replace("\\", "/"));
                        break;
                    }
                }
            }

            EditorGUI.showMixedValue = false;
        }

        protected virtual void Line2GUI()
        {
            if (childSerializedType == null)
                return;

            {
                string label = TryGetText("gui.pitch");
                BeginLabelWidth(label);

                childSerializedType?.GetProperty(nameof(SoundMetaDataBase.pitch))?.DrawGUILayout(label);
                EndLabelWidth();
            }

            Space(5);

            {
                string label = TryGetText("gui.tempo");
                BeginLabelWidth(label);

                childSerializedType?.GetProperty(nameof(SoundMetaDataBase.tempo))?.DrawGUILayout(label);
                EndLabelWidth();
            }
        }

        /// <summary>이 메소드는 내부 호출 시에 <see cref="EditorGUILayout.BeginHorizontal(GUILayoutOption[])"/> 메소드가 호출되지 않습니다</summary>
        protected virtual void LineOtherGUI() { }

        protected override float InternalGetPropertyHeight()
        {
            if (property.canRead && property.GetValue() == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
