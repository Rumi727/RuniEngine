#nullable enable
using RuniEngine.Editor.Inspector;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraSetter))]
    public class CameraSetterEditor : CustomInspectorBase<CameraSetter>
    {
        AnimBool? disableAnim;
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            UseProperty("_disable", TryGetText("gui.disable"));

            FadeGroup(ref disableAnim, !target.disable || !TargetsIsEquals(x => x.disable), () =>
            {
                Space();

                EditorGUI.BeginChangeCheck();

                UseProperty("_normalizedViewPortRect", TryGetText("inspector.camera_setter.normalizedViewPortRect"));

                Space();

                UseProperty("_globalScreenPositionMultiple", TryGetText("inspector.camera_setter.globalScreenPositionMultiple"));
                UseProperty("_globalScreenAreaMultiple", TryGetText("inspector.camera_setter.globalScreenAreaMultiple"));

                if (EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        CameraSetter? cameraSetter = targets[i];
                        if (cameraSetter != null)
                            cameraSetter.SetDirty();
                    }
                }
            });
        }
    }
}
