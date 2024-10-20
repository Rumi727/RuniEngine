using RuniEngine.UI.Animating;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    public abstract class UIAnimationEditor<TTarget> : UIBaseEditor<TTarget> where TTarget : UIAnimation
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null)
                return;

            if (!TargetsIsEquals(x => x.animator))
                EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();
            UIAnimator selectedAnimator = (UIAnimator)EditorGUILayout.ObjectField(TryGetText("inspector.ui_animation.animator"), target.animator, typeof(UIAnimator), true);

            if (EditorGUI.EndChangeCheck())
            {
                List<(UIAnimator animator, UIAnimation animation)> removes = new List<(UIAnimator animator, UIAnimation animation)>();
                List<(UIAnimator animator, UIAnimation animation)> adds = new List<(UIAnimator animator, UIAnimation animation)>();
                List<Object> undos = new List<Object>();

                TargetsInvoke(x =>
                {
                    if (x.animator != null)
                    {
                        removes.Add((x.animator, target));
                        undos.Add(x.animator);
                    }

                    if (selectedAnimator != null)
                    {
                        adds.Add((selectedAnimator, target));
                        undos.Add(selectedAnimator);
                    }
                });

                Undo.RecordObjects(undos.ToArray(), TryGetText("undo.modified_~").Replace("{object}", TryGetText("inspector.ui_animation.animator")));

                for (int i = 0; i < removes.Count; i++)
                {
                    (UIAnimator animator, UIAnimation animation) = removes[i];
                    animator.animations.Remove(animation);
                }

                for (int i = 0; i < adds.Count; i++)
                {
                    (UIAnimator animator, UIAnimation animation) = adds[i];

                    animator.animations.Add(animation);
                    animation.Init(animator);
                }
            }

            EditorGUI.showMixedValue = false;
            GUILayout.Label($"{TryGetText("gui.length")}: {TargetsToString(x => x.length)}");
        }

        public void AutoField(ref AnimBool? animBool, bool condition, string propertyName, string label) => FadeGroup(ref animBool, condition, () => UseProperty(propertyName, label));
    }
}
