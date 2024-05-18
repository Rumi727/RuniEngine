#nullable enable
using RuniEngine.APIBridge.UnityEngine;
using System;
using System.Reflection;

namespace RuniEngine.Editor.APIBridge.UnityEditor
{
    public sealed class EditorGUIUtility : GUIUtility
    {
        public static new Type type { get; } = typeof(global::UnityEditor.EditorGUIUtility);

        EditorGUIUtility() { }

        public static int s_LastControlID
        {
            get
            {
                f_s_LastControlID ??= type.GetField("s_LastControlID", BindingFlags.NonPublic | BindingFlags.Static);
                return (int)f_s_LastControlID.GetValue(null);
            }
            set
            {
                f_s_LastControlID ??= type.GetField("s_LastControlID", BindingFlags.NonPublic | BindingFlags.Static);
                f_s_LastControlID.SetValue(null, value);
            }
        }
        static FieldInfo? f_s_LastControlID;
    }
}
