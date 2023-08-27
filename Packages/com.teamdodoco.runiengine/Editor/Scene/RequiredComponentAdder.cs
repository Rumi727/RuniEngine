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
                AddComponent<RectTransform, RectTransformTool>(prefabStage);
            }
            finally
            {
                hierarchyChangedDisable = false;
            }
        }

        public static TComponentToAdd[] AddComponent<TComponent, TComponentToAdd>(PrefabStage prefabStage) where TComponent : Component where TComponentToAdd : Behaviour
        {
            TComponent[] components;
            if (prefabStage != null)
                components = prefabStage.FindComponentsOfType<TComponent>();
            else
                components = Object.FindObjectsOfType<TComponent>(true);

            List<TComponentToAdd> results = new List<TComponentToAdd>();
            for (int i = 0; i < components.Length; i++)
            {
                TComponent component = components[i];

                if (!component.TryGetComponent<TComponentToAdd>(out TComponentToAdd? componentToAdd))
                {
                    TComponentToAdd? addedComponent = EditorTool.AddComponentCompatibleWithPrefab<TComponentToAdd>(component.gameObject, true);
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