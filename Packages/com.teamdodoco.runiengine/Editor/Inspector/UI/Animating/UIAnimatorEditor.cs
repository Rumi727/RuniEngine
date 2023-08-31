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
                {
                    serializedObject.ApplyModifiedProperties();
                    for (int i = 0; i < targets.Length; i++)
                        targets[i].LayoutUpdate();
                }
            }
            else
                UseProperty("_time", TryGetText("gui.time"));

            GUILayout.Label($"{TryGetText("gui.length")}: {TargetsToString(x => x.length)}");

            Space();

            UseProperty("_animations", TryGetText("inspector.ui_animator.animations"));

            Space();

            UseProperty("_playOnAwake", TryGetText("gui.play_on_awake"));
            if (TargetsIsEquals(x => x.playOnAwake))
            {
                if (target.playOnAwake)
                    UseProperty("_playOnAwakeDelay", TryGetText("gui.play_on_awake_delay"));
            }
            else
                UseProperty("_playOnAwakeDelay", TryGetText("gui.play_on_awake_delay"));

            Space();

            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(TryGetText("gui.play")))
                {
                    for (int i = 0; i < targets.Length; i++)
                        targets[i].Play();
                }

                if (GUILayout.Button(TryGetText("gui.reverse")))
                {
                    for (int i = 0; i < targets.Length; i++)
                        targets[i].Reverse();
                }

                if (!TargetsIsEquals(x => x.isPlaying) || !target.isPlaying)
                {
                    if (GUILayout.Button(TryGetText("gui.unpause")))
                    {
                        for (int i = 0; i < targets.Length; i++)
                            targets[i].UnPause();
                    }
                }
                else
                {
                    if (GUILayout.Button(TryGetText("gui.pause")))
                    {
                        for (int i = 0; i < targets.Length; i++)
                            targets[i].Pause();
                    }
                }

                if (GUILayout.Button(TryGetText("gui.stop")))
                {
                    for (int i = 0; i < targets.Length; i++)
                        targets[i].Stop();
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}
