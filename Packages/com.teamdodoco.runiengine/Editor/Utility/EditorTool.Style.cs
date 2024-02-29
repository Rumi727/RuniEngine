#nullable enable
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static GUIStyle labelButtonStyle
        {
            get
            {
                _labelButtonStyle ??= new GUIStyle(EditorStyles.label)
                {
                    hover = new GUIStyleState()
                    {
                        textColor = new Color(0, 0.2352941176f, 0.5333333333f)
                    },
                    active = new GUIStyleState()
                    {
                        textColor = new Color(0, 0.2352941176f * 2, 0.5333333333f * 2)
                    }
                };
                
                return _labelButtonStyle;
            }
        }
        static GUIStyle? _labelButtonStyle;



        public static void BeginLabelWidth(string label) => BeginLabelWidth(new GUIContent(label));
        public static void BeginLabelWidth(GUIContent label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(string label, GUIStyle style) => BeginLabelWidth(new GUIContent(label), style);
        public static void BeginLabelWidth(GUIContent label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);

        public static void BeginLabelWidth(params string[] label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(params GUIContent[] label) => BeginLabelWidth(label, EditorStyles.label);
        public static void BeginLabelWidth(string[] label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);
        public static void BeginLabelWidth(GUIContent[] label, GUIStyle style) => BeginLabelWidth(GetLabelXSize(label, style) + 2);

        static readonly Stack<float> labelWidthQueue = new Stack<float>();
        public static void BeginLabelWidth(float width)
        {
            labelWidthQueue.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = width;
        }

        public static void EndLabelWidth()
        {
            if (labelWidthQueue.TryPop(out float result))
                EditorGUIUtility.labelWidth = result;
            else
                EditorGUIUtility.labelWidth = 0;
        }

        public static float GetLabelXSize(string label) => GetLabelXSize(new GUIContent(label));
        public static float GetLabelXSize(GUIContent label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(string label, GUIStyle style) => GetLabelXSize(new GUIContent(label), style);
        public static float GetLabelXSize(GUIContent label, GUIStyle style) => style.CalcSize(label).x;

        public static float GetLabelXSize(params string[] label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(params GUIContent[] label) => GetLabelXSize(label, EditorStyles.label);
        public static float GetLabelXSize(string[] label, GUIStyle style)
        {
            float width = 0;
            for (int i = 0; i < label.Length; i++)
                width = width.Max(style.CalcSize(new GUIContent(label[i])).x);

            return width;
        }

        public static float GetLabelXSize(GUIContent[] label, GUIStyle style)
        {
            float width = 0;
            for (int i = 0; i < label.Length; i++)
                width = width.Max(style.CalcSize(label[i]).x);

            return width;
        }

        static readonly Stack<float> fieldWidthQueue = new Stack<float>();
        public static void BeginFieldWidth(float width)
        {
            fieldWidthQueue.Push(EditorGUIUtility.fieldWidth);
            EditorGUIUtility.fieldWidth = width;
        }

        public static void EndFieldWidth()
        {
            if (fieldWidthQueue.TryPop(out float result))
                EditorGUIUtility.fieldWidth = result;
            else
                EditorGUIUtility.fieldWidth = 0;
        }



        public static void BeginFontSize(int size) => BeginFontSize(size, EditorStyles.label);
        public static void EndFontSize() => EndFontSize(EditorStyles.label);

        static readonly Dictionary<GUIStyle, Stack<int>> fontSizeQueue = new Dictionary<GUIStyle, Stack<int>>();
        public static void BeginFontSize(int size, GUIStyle style)
        {
            if (!fontSizeQueue.ContainsKey(style))
                fontSizeQueue.Add(style, new Stack<int>());

            fontSizeQueue[style].Push(style.fontSize);
            style.fontSize = size;
        }

        public static void EndFontSize(GUIStyle style)
        {
            if (fontSizeQueue.ContainsKey(style))
            {
                Stack<int> stack = fontSizeQueue[style];
                if (stack.TryPop(out int result))
                    style.fontSize = result;
                else
                    style.fontSize = 0;

                if (stack.Count <= 0)
                    fontSizeQueue.Remove(style);

                return;
            }
            else
                style.fontSize = 0;
        }
    }
}
