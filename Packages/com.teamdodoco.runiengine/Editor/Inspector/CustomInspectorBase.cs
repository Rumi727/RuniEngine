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
        protected new TTarget[]? targets { get; private set; }

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



        public SerializedProperty? UseProperty(string propertyName) => UseProperty(serializedObject, propertyName);
        public SerializedProperty? UseProperty(string propertyName, string label) => UseProperty(serializedObject, propertyName, label);



        public bool TargetsIsEquals<TValue>(Func<TTarget, TValue> func) => TargetsIsEquals(targets, func);

        public string TargetsToString<TValue>(Func<TTarget, TValue> func) => TargetsToString(targets, func);
    }
}
