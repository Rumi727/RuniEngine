#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void FileObjectField<T>(string label, ref string path, out bool isChanged) where T : Object
        {
            T oldAssets = AssetDatabase.LoadAssetAtPath<T>(path);
            T assets = (T)EditorGUILayout.ObjectField(label, oldAssets, typeof(T), false);

            path = AssetDatabase.GetAssetPath(assets);

            EditorGUILayout.LabelField(TryGetText("gui.path") + ": " + path);
            isChanged = oldAssets != assets;
        }
    }
}
