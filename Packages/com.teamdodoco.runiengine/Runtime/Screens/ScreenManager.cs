#nullable enable
using RuniEngine.Booting;
using UnityEngine;

namespace RuniEngine.Screens
{
    public static class ScreenManager
    {
        public static int width { get; private set; }
        public static int height { get; private set; }



        public static Vector2Int size => new Vector2Int(width, height);

        public static Resolution currentResolution { get; private set; } = new Resolution();
        public static Resolution[] resolutions { get; private set; } = new Resolution[0];



        public static Vector3 screenPosition
        {
            get
            {
                Vector3 result = Vector3.zero;

                for (int i = 0; i < ScreenMover.instances.Count; i++)
                {
                    Vector3 item = ScreenMover.instances[i].position;
                    result += item;
                }

                return result;
            }
        }

        public static RectOffset screenArea
        {
            get
            {
                if (ScreenCroper.instances.Count <= 0)
                    return new RectOffset();

                Vector2 min = new Vector2(float.MinValue, float.MinValue);
                Vector2 max = new Vector2(float.MaxValue, float.MaxValue);

                for (int i = 0; i < ScreenCroper.instances.Count; i++)
                {
                    RectOffset item = ScreenCroper.instances[i].offset;

                    min.x = min.x.Max(item.min.x);
                    min.y = min.y.Max(item.min.y);

                    max.x = max.x.Min(item.max.x);
                    max.y = max.y.Min(item.max.y);
                }

                return new RectOffset(min, max);
            }
        }



        [Awaken]
        static void Awaken()
        {
            CustomPlayerLoopSetter.initEvent += Update;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
            Kernel.quitting += () => UnityEditor.EditorApplication.update += Update;
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            if (!Kernel.isPlaying)
                UnityEditor.EditorApplication.update += Update;
        }
#endif

        static void Update()
        {
            if (Kernel.isPlaying)
            {
                width = Screen.width;
                height = Screen.height;
            }
            else
            {
#if UNITY_EDITOR
                string[] res = UnityEditor.UnityStats.screenRes.Split('x');
                width = int.Parse(res[0]);
                height = int.Parse(res[1]);
#endif
            }

            currentResolution = Screen.currentResolution;
            resolutions = Screen.resolutions;
        }
    }
}
