#nullable enable
using System;
using System.Reflection;
using UnityEditor.IMGUI.Controls;

using UniAdvancedDropdown = UnityEditor.IMGUI.Controls.AdvancedDropdown;

namespace RuniEngine.Editor.APIBridge.UnityEditor.IMGUI.Controls
{
    public class AdvancedDropdown
    {
        public static Type type { get; } = typeof(UniAdvancedDropdown);

        public static AdvancedDropdown GetInstance(UniAdvancedDropdown instance) => new AdvancedDropdown(instance);

        AdvancedDropdown(UniAdvancedDropdown instance) => this.instance = instance;

        public UniAdvancedDropdown instance { get; set; }

        public AdvancedDropdownState m_State
        {
            get
            {
                f_m_State ??= type.GetField("m_State", BindingFlags.NonPublic | BindingFlags.Instance);
                return (AdvancedDropdownState)f_m_State.GetValue(instance);
            }
            set
            {
                f_m_State ??= type.GetField("m_State", BindingFlags.NonPublic | BindingFlags.Instance);
                f_m_State.SetValue(instance, value);
            }
        }
        static FieldInfo? f_m_State;
    }
}
