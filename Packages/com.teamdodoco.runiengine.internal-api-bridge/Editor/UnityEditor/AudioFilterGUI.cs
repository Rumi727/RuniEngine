#nullable enable
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RuniEngine.Editor.APIBridge.UnityEditor
{
    public sealed class AudioFilterGUI
    {
        public static Type type { get; } = EditorAssemblyManager.UnityEditor_CoreModule.GetType("UnityEditor.AudioFilterGUI");

        public static AudioFilterGUI CreateInstance() => new AudioFilterGUI(Activator.CreateInstance(type));
        public static AudioFilterGUI GetInstance(object instance) => new AudioFilterGUI(instance);

        AudioFilterGUI(object instance) => this.instance = instance;

        public object instance { get; }



        public EditorGUI.VUMeter.SmoothingData[]? dataOut
        {
            get
            {
                f_dataOut ??= type.GetField("dataOut", BindingFlags.NonPublic | BindingFlags.Instance);
                return (EditorGUI.VUMeter.SmoothingData[]?)f_dataOut.GetValue(instance);
            }
            set
            {
                f_dataOut ??= type.GetField("dataOut", BindingFlags.Public | BindingFlags.Instance);
                f_dataOut.SetValue(instance, value);
            }
        }
        static FieldInfo? f_dataOut;



        static MethodInfo? m_DrawAudioFilterGUI;
        static readonly object[] mp_DrawAudioFilterGUI = new object[1];
        public void DrawAudioFilterGUI(MonoBehaviour monoBehaviour)
        {
            m_DrawAudioFilterGUI ??= type.GetMethod("DrawAudioFilterGUI", BindingFlags.Public | BindingFlags.Instance);

            mp_DrawAudioFilterGUI[0] = monoBehaviour;
            m_DrawAudioFilterGUI.Invoke(instance, mp_DrawAudioFilterGUI);
        }



        public override string ToString() => instance.ToString();
    }
}
