using System;
using System.Reflection;

namespace RuniEngine.APIBridge
{
    internal static class AssemblyManager
    {
        public static Assembly[] assemblys => _assemblys ??= AppDomain.CurrentDomain.GetAssemblies();
        static Assembly[]? _assemblys;
    }
}