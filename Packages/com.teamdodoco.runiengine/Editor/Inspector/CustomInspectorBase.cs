#nullable enable
using Cysharp.Threading.Tasks;
using System;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace RuniEngine.Editor.Inspector
{
    public class CustomInspectorBase<TTarget> : EditorTool where TTarget : Object 
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

        public override void OnInspectorGUI()
        {
            
        }



        public SerializedProperty? UseProperty(string propertyName) => InternalUseProperty(propertyName, "", false);
        public SerializedProperty? UseProperty(string propertyName, string label) => InternalUseProperty(propertyName, label, true);
        SerializedProperty? InternalUseProperty(string propertyName, string label, bool labelShow)
        {
            GUIContent? guiContent = null;
            if (labelShow)
                guiContent = new GUIContent { text = label };

            try
            {
                SerializedProperty tps = serializedObject.FindProperty(propertyName);
                EditorGUI.BeginChangeCheck();

                if (!labelShow)
                    EditorGUILayout.PropertyField(tps, true);
                else
                    EditorGUILayout.PropertyField(tps, guiContent, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                return tps;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                GUILayout.Label($@"프로퍼티 '{propertyName}'을(를) 인식하지 못했습니다");
            }

            return null;
        }

        public string TargetsToString<TValue>(Func<TTarget, TValue> action)
        {
            if (targets == null || targets.Length <= 0)
                return "null";

            TValue? parentValue = action(targets[0]);
            for (int i = 1; i < targets.Length; i++)
            {
                TValue value = action(targets[i]);
                if (!Equals(parentValue, value))
                    return "-";

                parentValue = value;
            }

            if (parentValue != null)
                return parentValue.ToString();
            else
                return "null";
        }
    }
}
