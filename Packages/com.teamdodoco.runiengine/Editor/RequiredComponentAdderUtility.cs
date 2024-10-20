using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor
{
    public static class RequiredComponentAdderUtility
    {
        public static TComponentToAdd[] AddComponent<TComponent, TComponentToAdd>(PrefabStage? prefabStage, bool backToTop, string ignoreScenePath = "") where TComponent : Component where TComponentToAdd : Behaviour
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
                if (!string.IsNullOrEmpty(ignoreScenePath) && component.gameObject.scene.path.StartsWith(ignoreScenePath))
                    continue;

                if (!component.TryGetComponent<TComponentToAdd>(out TComponentToAdd? componentToAdd))
                {
                    TComponentToAdd? addedComponent = AddComponentCompatibleWithPrefab<TComponentToAdd>(component.gameObject, backToTop);
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
