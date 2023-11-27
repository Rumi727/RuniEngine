#nullable enable
using UnityEngine;

namespace RuniEngine
{
    public static partial class Kernel
    {
        public static float fps { get; private set; } = 60;

        public static float deltaTime { get; private set; } = fps60second;
        public static float fpsDeltaTime { get; private set; } = 1;

        public static double deltaTimeDouble { get; private set; } = fps60second;
        public static double fpsDeltaTimeDouble { get; private set; } = 1;

        public static float smoothDeltaTime { get; private set; } = fps60second;
        public static float fpsSmoothDeltaTime { get; private set; } = fps60second;

        public static float unscaledDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledDeltaTime { get; private set; } = 1;

        public static double unscaledDeltaTimeDouble { get; private set; } = fps60second;
        public static double fpsUnscaledDeltaTimeDouble { get; private set; } = 1;

        public static float unscaledSmoothDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledSmoothDeltaTime { get; private set; } = fps60second;

        public static float fixedDeltaTime { get; set; } = fps60second;

        public const float fps60second = 1f / 60f;

        /// <summary>
        /// 게임의 전체 속도를 결정 합니다
        /// </summary>
        public static float gameSpeed { get; set; } = 1;

        static void TimeUpdate()
        {
            double realDeltaTime = deltaTimeStopwatch.Elapsed.TotalSeconds;
            deltaTimeStopwatch.Restart();

            //게임 속도를 0에서 100 사이로 정하고, 타임 스케일을 게임 속도로 정합니다
            gameSpeed = gameSpeed.Clamp(0, 100);
            Time.timeScale = gameSpeed;

            //유니티의 내장 변수들은 테스트 결과, 약간의 성능을 더 먹는것으로 확인되었기 때문에
            //이렇게 관리 스크립트가 변수를 할당하고 다른 스크립트가 그 변수를 가져오는것이 성능에 더 도움 되는것을 확인하였습니다
            deltaTime = (float)realDeltaTime * gameSpeed;
            fpsDeltaTime = (float)(deltaTime * VideoManager.standardFPS);

            deltaTimeDouble = realDeltaTime * gameSpeed;
            fpsDeltaTimeDouble = deltaTimeDouble * VideoManager.standardFPS;

            float lastUnscaledDeltaTime = unscaledDeltaTime;
            unscaledDeltaTime = (float)realDeltaTime;
            fpsUnscaledDeltaTime = (float)(unscaledDeltaTime * VideoManager.standardFPS);

            unscaledDeltaTimeDouble = realDeltaTime;
            fpsUnscaledDeltaTimeDouble = unscaledDeltaTimeDouble * VideoManager.standardFPS;

            fixedDeltaTime = (float)(1d / VideoManager.standardFPS);
            Time.fixedDeltaTime = fixedDeltaTime;

            fps = 1f / unscaledDeltaTime;

            //Smooth Delta Time
            //테스트 결과, 게임 속도 영향을 제외하면 유니티 내부 구현이랑 정확히 같습니다
            {
                unscaledSmoothDeltaTime += (unscaledDeltaTime - lastUnscaledDeltaTime) * 0.2f;
                smoothDeltaTime = unscaledDeltaTime * gameSpeed;

                fpsSmoothDeltaTime = (float)(smoothDeltaTime * VideoManager.standardFPS);
                fpsUnscaledSmoothDeltaTime = (float)(unscaledSmoothDeltaTime * VideoManager.standardFPS);
            }
        }
    }
}
