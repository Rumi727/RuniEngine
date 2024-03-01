#nullable enable
using RuniEngine.UI;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public static class AutoSceneVisibilitySetter
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            Selection.selectionChanged += Update;
            Update();
        }

        public static void Update()
        {
            CanvasSetter[] canvasSettings = Object.FindObjectsByType<CanvasSetter>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < canvasSettings.Length; i++)
            {
                CanvasSetter canvasSetting = canvasSettings[i];
                if (canvasSetting.alwaysVisible)
                    SceneVisibilityManager.instance.Show(canvasSetting.gameObject, true);
                else
                    SceneVisibilityManager.instance.Hide(canvasSetting.gameObject, true);
            }

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Canvas canvas = Selection.gameObjects[i].GetComponentInParent<Canvas>(true);
                if (canvas != null)
                    SceneVisibilityManager.instance.Show(canvas.gameObject, true);
            }
        }
    }
}
