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
                
                IList? list = (IList?)f_dataOut.GetValue(instance);
                if (list == null)
                    return null;

                if (list.Count != tg_dataOut?.Length)
                    tg_dataOut = new EditorGUI.VUMeter.SmoothingData[list.Count];

                for (int i = 0; i < list.Count; i++)
                    tg_dataOut[i] = EditorGUI.VUMeter.SmoothingData.GetInstance(list[i]);

                return tg_dataOut;
            }
            set
            {
                f_dataOut ??= type.GetField("dataOut", BindingFlags.Public | BindingFlags.Instance);
                if (value == null)
                {
                    f_dataOut.SetValue(instance, null);
                    tg_dataOut = null;

                    return;
                }

                if (value.Length != ts_dataOut?.Length)
                    ts_dataOut = new object[value.Length];

                for (int i = 0; i < value.Length; i++)
                    ts_dataOut[i] = value[i].instance;

                f_dataOut.SetValue(instance, ts_dataOut);
            }
        }
        EditorGUI.VUMeter.SmoothingData[]? tg_dataOut;
        object[]? ts_dataOut;
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
