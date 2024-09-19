#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RuniEngine
{
    public static class InfiniteLoopDetector
    {
        static int detectionCount = 0;
        const int detectionThreshold = 1000000;

        [Conditional("UNITY_EDITOR")]
        public static void Run([CallerFilePath] string fp = "", [CallerLineNumber] int ln = 0, [CallerMemberName] string mn = "")
        {
            string currentPoint = $"{fp}:{ln}, {mn}()";
            if (RunWithoutException())
                throw new Exception($"Infinite Loop Detected: {currentPoint}");
        }

        public static bool RunWithoutException()
        {
#if UNITY_EDITOR
            if (Interlocked.Add(ref detectionCount, 1) > detectionThreshold)
                return true;
#endif
            return false;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void Init() => UnityEditor.EditorApplication.update += () => Interlocked.Exchange(ref detectionCount, 0);
/*#else
        [Awaken]
        static void Awaken() => CustomPlayerLoopSetter.initEvent += () => Interlocked.Exchange(ref detectionCount, 0);*/
#endif
    }
}