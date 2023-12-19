#nullable enable
using RuniEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using SceneManager = RuniEngine.SceneManagement.SceneManager;

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

                {
                    Scene scene = SceneManager.GetActiveScene();
                    if (scene.buildIndex == 0 || scene.buildIndex == 1)
                        return;
                }

                PrefabStage? prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                RequiredComponentAdderUtility.AddComponent<RectTransform, RectTransformTool>(prefabStage, true);
                RequiredComponentAdderUtility.AddComponent<Canvas, CanvasSetter>(prefabStage, false);
            }
            finally
            {
                hierarchyChangedDisable = false;
            }
        }
    }
}
