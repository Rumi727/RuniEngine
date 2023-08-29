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



        public SerializedProperty? UseProperty(string propertyName) => InternalUseProperty(propertyName, "", false);
        public SerializedProperty? UseProperty(string propertyName, string label) => InternalUseProperty(propertyName, label, true);
        SerializedProperty? InternalUseProperty(string propertyName, string label, bool labelShow)
        {
            GUIContent? guiContent = null;
            if (labelShow)
                guiContent = new GUIContent { text = label };

            SerializedProperty? tps = null;

            try
            {
                tps = serializedObject.FindProperty(propertyName);
            }
            catch (ExitGUIException)
            {
                
            }
            catch (Exception)
            {
                GUILayout.Label(TryGetText("inspector.property_none").Replace("{name}", propertyName));
                return null;
            }

            if (tps != null)
            {
                EditorGUI.BeginChangeCheck();

                if (!labelShow)
                    EditorGUILayout.PropertyField(tps, true);
                else
                    EditorGUILayout.PropertyField(tps, guiContent, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            return tps;
        }

        public bool TargetsIsEquals<TValue>(Func<TTarget, TValue> func)
        {
            if (targets == null || targets.Length <= 0)
                return true;

            TValue? parentValue = func(targets[0]);
            for (int i = 1; i < targets.Length; i++)
            {
                TValue value = func(targets[i]);
                if (!Equals(parentValue, value))
                    return false;

                parentValue = value;
            }

            return true;
        }

        public string TargetsToString<TValue>(Func<TTarget, TValue> func)
        {
            if (targets == null || targets.Length <= 0)
                return "null";

            if (!TargetsIsEquals(func))
                return "-";

            TValue value = func(targets[0]);
            if (value != null)
                return value.ToString();
            else
                return "null";
        }
    }
}
