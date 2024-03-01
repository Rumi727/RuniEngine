#nullable enable
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RuniEngine.Editor
{
    [InitializeOnLoad]
    public static class RequiredComponentAdder
    {
        static RequiredComponentAdder()
        {
            EditorApplication.hierarchyChanged += Refresh;
            Refresh();
        }

        static bool hierarchyChangedDisable = false;
        public static void Refresh()
        {
            if (hierarchyChangedDisable || Kernel.isPlaying)
                return;

            try
            {
                hierarchyChangedDisable = true;

                PrefabStage? prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                RequiredComponentAdderUtility.AddComponent<Camera, CameraSetter>(prefabStage, false);
            }
            finally
            {
                hierarchyChangedDisable = false;
            }
        }
    }
}
