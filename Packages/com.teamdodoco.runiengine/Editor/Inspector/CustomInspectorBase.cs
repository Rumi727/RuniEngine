#nullable enable
using Cysharp.Threading.Tasks;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuniEngine.Editor.Inspector
{
    public abstract class CustomInspectorBase<TTarget> : EditorTool where TTarget : Object
    {
        protected new TTarget? target { get; private set; }
        protected new TTarget?[]? targets { get; private set; }

        [NonSerialized] bool repaint = false;

        /// <summary>
        /// Please put base.OnEnable() when overriding
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Kernel.isPlaying)
            {
                repaint = true;
                Repainter();
            }

            target = (TTarget)base.target;
            targets = ConverterUtility.ConvertObjects<TTarget>(base.targets);
        }

        /// <summary>
        /// Please put base.OnDisable() when overriding
        /// </summary>
        protected virtual void OnDisable() => repaint = false;

        async void Repainter()
        {
            while (repaint)
            {
                Repaint();
                await UniTask.Delay(100, true);
            }
        }



        public SerializedProperty? UseProperty(string propertyName, params GUILayoutOption[] options) => UseProperty(serializedObject, propertyName, options);
        public SerializedProperty? UseProperty(string propertyName, string label, params GUILayoutOption[] options) => UseProperty(serializedObject, propertyName, label, options);



        public bool TargetsIsEquals<TValue>(Func<TTarget, TValue> func) => TargetsIsEquals(func, targets);

        public string TargetsToString<TValue>(Func<TTarget, TValue> func) => TargetsToString(func, targets);


        public void TargetsInvoke(Action<TTarget> action)
        {
            if (targets == null || targets.Length <= 0)
                return;

            for (int i = 0; i < targets.Length; i++)
            {
                TTarget? target2 = targets[i];
                if (target2 != null)
                    action.Invoke(target2);
            }
        }
    }
}
