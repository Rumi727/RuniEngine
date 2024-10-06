using System;

namespace RuniEngine.APIBridge.UnityEngine
{
    public class GUIUtility
    {
        public static Type type { get; } = typeof(global::UnityEngine.GUIUtility);

        protected GUIUtility() { }
    }
}
