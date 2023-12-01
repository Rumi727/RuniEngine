#nullable enable
using RuniEngine.Editor.Inspector.UI;
using RuniEngine.UI;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CanvasSetter))]
    public class CanvasSetterEditor : UIBaseEditor<CanvasSetter>
    {
        public override void OnInspectorGUI()
        {
            if (targets == null || targets.Length <= 0 || target == null)
                return;

            bool mixed = targets.Length > 1;

            EditorGUI.BeginChangeCheck();

            UseProperty("_disableScreenArea", TryGetText("inspector.canvas_setter.disableScreenArea"));

            if (!target.disableScreenArea || mixed)
            {
                UseProperty("_worldRenderMode", TryGetText("inspector.canvas_setter.worldRenderMode"));

                if (target.worldRenderMode || mixed)
                    UseProperty("_planeDistance", TryGetText("inspector.canvas_setter.planeDistance"));

                Space();

                UseProperty("_areaOffset", TryGetText("inspector.canvas_setter.areaOffset"));

                Space();

                UseProperty("_globalScreenPositionMultiple", TryGetText("inspector.canvas_setter.globalScreenPositionMultiple"));
                UseProperty("_globalScreenAreaMultiple", TryGetText("inspector.canvas_setter.globalScreenAreaMultiple"));
            }

            DrawLine();

            UseProperty("_disableUISize", TryGetText("inspector.canvas_setter.disableUISize"));
            if (!target.disableUISize || mixed)
                UseProperty("_uiSize", TryGetText("inspector.canvas_setter.uiSize"));

            DrawLine();

            UseProperty("_alwaysVisible", TryGetText("inspector.canvas_setter.alwaysVisible"));

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    CanvasSetter? canvasSetter = targets[i];
                    if (canvasSetter != null)
                        canvasSetter.SetDirty();
                }
            }

            DrawLine();

            if (GUILayout.Button(TryGetText("gui.delete"), GUILayout.ExpandWidth(false)) && targets != null)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    CanvasSetter? tool = targets[i];
                    if (tool == null)
                        continue;

                    Canvas? canvas = tool.canvas;

                    DestroyImmediate(tool);
                    Undo.DestroyObjectImmediate(canvas);
                }
            }
        }
    }
}