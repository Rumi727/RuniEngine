using System;
using System.Reflection;

namespace RuniEngine.Editor.APIBridge.UnityEditor
{
    public sealed class EditorGUI
    {
        public static Type type { get; } = typeof(global::UnityEditor.EditorGUI);

        EditorGUI() { }

        public class VUMeter
        {
            public static Type type { get; } = EditorAssemblyManager.UnityEditor_CoreModule.GetType("UnityEditor.EditorGUI+VUMeter");

            VUMeter() { }

            public struct SmoothingData
            {
                public static Type type { get; } = EditorAssemblyManager.UnityEditor_CoreModule.GetType("UnityEditor.EditorGUI+VUMeter+SmoothingData");

                public static SmoothingData CreateInstance() => new SmoothingData(Activator.CreateInstance(type));
                public static SmoothingData GetInstance(object instance) => new SmoothingData(instance);

                SmoothingData(object instance)
                {
                    this.instance = instance;

                    f_lastValue = null;
                    f_peakValue = null;
                    f_peakValueTime = null;
                }

                public object instance { get; }



                public float lastValue
                {
                    get
                    {
                        f_lastValue ??= type.GetField("lastValue", BindingFlags.Public | BindingFlags.Instance);
                        return (float)f_lastValue.GetValue(instance);
                    }
                    set
                    {
                        f_lastValue ??= type.GetField("lastValue", BindingFlags.Public | BindingFlags.Instance);
                        f_lastValue.SetValue(instance, value);
                    }
                }
                FieldInfo? f_lastValue;

                public float peakValue
                {
                    get
                    {
                        f_peakValue ??= type.GetField("peakValue", BindingFlags.Public | BindingFlags.Instance);
                        return (float)f_peakValue.GetValue(instance);
                    }
                    set
                    {
                        f_peakValue ??= type.GetField("peakValue", BindingFlags.Public | BindingFlags.Instance);
                        f_peakValue.SetValue(instance, value);
                    }
                }
                FieldInfo? f_peakValue;

                public float peakValueTime
                {
                    get
                    {
                        f_peakValueTime ??= type.GetField("peakValueTime", BindingFlags.Public | BindingFlags.Instance);
                        return (float)f_peakValueTime.GetValue(instance);
                    }
                    set
                    {
                        f_peakValueTime ??= type.GetField("peakValueTime", BindingFlags.Public | BindingFlags.Instance);
                        f_peakValueTime.SetValue(instance, value);
                    }
                }
                FieldInfo? f_peakValueTime;



                public override readonly string ToString() => instance.ToString();
            }
        }
    }
}
