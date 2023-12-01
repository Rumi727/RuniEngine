#nullable enable
using RuniEngine.Booting;
using UnityEngine;

namespace RuniEngine.Screens
{
    public static class ScreenManager
    {
        static int _width;
        public static int width
        {
            get
            {
                if (Kernel.isPlaying)
                    return _width;
                else
                    return Screen.width;
            }
        }

        static int _height;
        public static int height
        {
            get
            {
                if (Kernel.isPlaying)
                    return _height;
                else
                    return Screen.height;
            }
        }



        public static Vector2Int size => new Vector2Int(width, height);

        static Resolution _currentResolution;
        public static Resolution currentResolution
        {
            get
            {
                if (Kernel.isPlaying)
                    return _currentResolution;
                else
                    return Screen.currentResolution;
            }
        }

        static Resolution[] _resolutions = new Resolution[0];
        public static Resolution[] resolutions
        {
            get
            {
                if (Kernel.isPlaying)
                    return _resolutions;
                else
                    return Screen.resolutions;
            }
        }



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
                Vector2 min = Vector2.zero;
                Vector2 max = Vector2.zero;

                for (int i = 0; i < ScreenCroper.instances.Count; i++)
                {
                    RectOffset item = ScreenCroper.instances[i].offset;

                    min.x = min.x.Max(item.min.x);
                    min.y = min.x.Max(item.min.y);
                    
                    max.x = max.x.Max(item.max.x);
                    max.y = max.x.Max(item.max.y);
                }

                return new RectOffset(min, max);
            }
        }



        [Awaken]
        static void Awaken() => CustomPlayerLoopSetter.initEvent += Update;

        static void Update()
        {
            _width = Screen.width;
            _height = Screen.height;

            _currentResolution = Screen.currentResolution;
            _resolutions = Screen.resolutions;
        }
    }
}
