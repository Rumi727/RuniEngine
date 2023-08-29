#nullable enable
using RuniEngine.UI.Animating;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimator))]
    public class UIAnimatorEditor : UIBaseEditor<UIAnimator>
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            if (TargetsIsEquals(x => x.length))
            {
                SerializedProperty tps = serializedObject.FindProperty("_time");
                float time = target.time.Clamp(0, target.length);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Slider(tps, 0, target.length, TryGetText("gui.time"));

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
            else
                UseProperty("_time", TryGetText("gui.time"));

            GUILayout.Label($"{TryGetText("gui.length")}: {TargetsToString(x => x.length)}");

            Space();

            UseProperty("_animations", TryGetText("inspector.ui_animator.animations"));
        }
    }
}
