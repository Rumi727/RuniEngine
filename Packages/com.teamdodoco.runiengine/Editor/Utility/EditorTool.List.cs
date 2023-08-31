#nullable enable
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static void DeleteSafety(ref bool value)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(TryGetText("gui.delete_safety"), GUILayout.Width(330));
            value = EditorGUILayout.Toggle(value);

            EditorGUILayout.EndHorizontal();
        }
    }
}
