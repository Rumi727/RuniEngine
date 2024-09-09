#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RuniEngine
{
    /// <summary> 무한 루프 검사 및 방지 (에디터 전용)</summary>
    public static class InfiniteLoopDetector
    {
        static string prevPoint = "";
        static int detectionCount = 0;
        const int detectionThreshold = 1000000;

        [Conditional("UNITY_EDITOR")]
        public static void Run([CallerFilePath] string fp = "", [CallerLineNumber] int ln = 0, [CallerMemberName] string mn = "")
        {
            string currentPoint = $"{fp}:{ln}, {mn}()";
            int count;

            if (prevPoint == currentPoint)
                count = Interlocked.Add(ref detectionCount, 1);
            else
                count = Interlocked.Exchange(ref detectionCount, 0);

            if (count > detectionThreshold)
                throw new Exception($"Infinite Loop Detected: {currentPoint}");

            prevPoint = currentPoint;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void Init() => UnityEditor.EditorApplication.update += () => Interlocked.Exchange(ref detectionCount, 0);
#endif
    }
}