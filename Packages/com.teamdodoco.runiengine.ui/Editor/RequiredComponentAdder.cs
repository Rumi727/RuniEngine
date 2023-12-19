#nullable enable
using RuniEngine.UI;
using System.Collections.Generic;
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

                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
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
