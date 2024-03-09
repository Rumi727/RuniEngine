#nullable enable
using RuniEngine.APIBridge;
using System.Linq;
using System.Reflection;

namespace RuniEngine.Editor.APIBridge
{
    public sealed class EditorAssemblyManager : AssemblyManager
    {
        public static Assembly UnityEditor_CoreModule => _UnityEditor_CoreModule ??= assemblys.First(x => x.GetName().Name == "UnityEditor.CoreModule");
        static Assembly? _UnityEditor_CoreModule;

        public static Assembly UnityEditor_UI => _UnityEditor_UI ??= assemblys.First(x => x.GetName().Name == "UnityEditor.UI");
        static Assembly? _UnityEditor_UI;
    }
}
