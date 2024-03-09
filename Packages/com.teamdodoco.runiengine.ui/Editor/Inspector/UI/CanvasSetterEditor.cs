#nullable enable
using RuniEngine.Editor.Inspector.UI;
using RuniEngine.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CanvasSetter))]
    public class CanvasSetterEditor : UIBaseEditor<CanvasSetter>
    {
        readonly AnimBool disableScreenAreaAnim = new AnimBool();
        readonly AnimBool worldRenderModeAnim = new AnimBool();
        readonly AnimBool overlaySpaceAnim = new AnimBool();
        readonly AnimBool disableUISizeAnim = new AnimBool();

        public override void OnInspectorGUI()
        {
            if (targets == null || targets.Length <= 0 || target == null)
                return;

            bool mixed = targets.Length > 1;

            bool spaceMixed = !TargetsIsEquals(x => x.canvas != null ? x.canvas.renderMode : RenderMode.ScreenSpaceOverlay);

            bool overlaySpace = (target.canvas != null ? target.canvas.renderMode : RenderMode.ScreenSpaceOverlay) == RenderMode.ScreenSpaceOverlay;
            bool worldSpace = (target.canvas != null ? target.canvas.renderMode : RenderMode.ScreenSpaceOverlay) == RenderMode.WorldSpace;

            EditorGUI.BeginChangeCheck();

            {
                UseProperty("_disableScreenArea", TryGetText("inspector.canvas_setter.disableScreenArea"));

                disableScreenAreaAnim.FadeGroup(!target.disableScreenArea || !TargetsIsEquals(x => x.disableScreenArea, targets), Repaint, () =>
                {
                    {
                        UseProperty("_worldRenderMode", TryGetText("inspector.canvas_setter.worldRenderMode"));

                        worldRenderModeAnim.FadeGroup(target.worldRenderMode || !TargetsIsEquals(x => x.worldRenderMode), Repaint, () =>
                            UseProperty("_planeDistance", TryGetText("inspector.canvas_setter.planeDistance"))
                        );
                    }

                    Space();

                    {
                        UseProperty("_areaOffset", TryGetText("inspector.canvas_setter.areaOffset"));

                        overlaySpaceAnim.FadeGroup(overlaySpace || spaceMixed, Repaint, () =>
                        {
                            Space();

                            UseProperty("_globalScreenPositionMultiple", TryGetText("inspector.canvas_setter.globalScreenPositionMultiple"));
                            UseProperty("_globalScreenAreaMultiple", TryGetText("inspector.canvas_setter.globalScreenAreaMultiple"));
                        });
                    }
                });
            }

            DrawLine();

            {
                UseProperty("_disableUISize", TryGetText("inspector.canvas_setter.disableUISize"));

                disableUISizeAnim.FadeGroup
                (
                    !target.disableUISize && (!worldSpace || spaceMixed || target.worldRenderMode || !TargetsIsEquals(x => x.worldRenderMode)),
                    Repaint,
                    () =>
                    {
                        UseProperty("_uiSize", TryGetText("inspector.canvas_setter.uiSize"));

                        for (int i = 0; i < targets.Length; i++)
                        {
                            CanvasSetter? setter = targets[i];
                            if (setter == null)
                                continue;

                            if (setter.TryGetComponent<CanvasScaler?>(out _))
                            {
                                Space();
                                EditorGUILayout.HelpBox(TryGetText("inspector.canvas_setter_editor.error"), MessageType.Error);

                                break;
                            }
                        }
                    }
                );
            }

            DrawLine();

            UseProperty("_alwaysVisible", TryGetText("inspector.canvas_setter.alwaysVisible"));

            if (EditorGUI.EndChangeCheck())
                TargetsInvoke(x => x.SetDirty());

            DrawLine();

            if (GUILayout.Button(TryGetText("gui.delete"), GUILayout.ExpandWidth(false)) && targets != null)
            {
                TargetsInvoke(x =>
                {
                    Canvas? canvas = x.canvas;

                    DestroyImmediate(x);
                    Undo.DestroyObjectImmediate(canvas);
                });
            }
        }
    }
}