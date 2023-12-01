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
                AddComponent<RectTransform, RectTransformTool>(prefabStage, true);
                AddComponent<Canvas, CanvasSetter>(prefabStage, false);
            }
            finally
            {
                hierarchyChangedDisable = false;
            }
        }

        public static TComponentToAdd[] AddComponent<TComponent, TComponentToAdd>(PrefabStage prefabStage, bool backToTop) where TComponent : Component where TComponentToAdd : Behaviour
        {
            TComponent[] components;
            if (prefabStage != null)
                components = prefabStage.FindComponentsOfType<TComponent>();
            else
#if UNITY_2023_1_OR_NEWER
                components = Object.FindObjectsByType<TComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                components = Object.FindObjectsOfType<TComponent>(true);
#endif

            List<TComponentToAdd> results = new List<TComponentToAdd>();
            for (int i = 0; i < components.Length; i++)
            {
                TComponent component = components[i];

                if (!component.TryGetComponent<TComponentToAdd>(out TComponentToAdd? componentToAdd))
                {
                    TComponentToAdd? addedComponent = EditorTool.AddComponentCompatibleWithPrefab<TComponentToAdd>(component.gameObject, backToTop);
                    if (addedComponent != null)
                        results.Add(addedComponent);
                }
                else if (!componentToAdd.enabled)
                {
                    componentToAdd.enabled = true;
                    EditorUtility.SetDirty(componentToAdd);
                }
                else
                    results.Add(componentToAdd);
            }

            return results.ToArray();
        }
    }
}
