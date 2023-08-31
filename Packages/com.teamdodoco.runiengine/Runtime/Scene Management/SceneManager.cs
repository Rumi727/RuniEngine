#nullable enable
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace RuniEngine.SceneManagement
{
    public static class SceneManager
    {
        public static bool isDone { get; private set; } = true;
        public static bool isLoading { get; private set; } = false;

        public static event Action? activeSceneChanged;

        public static async UniTask LoadScene(int sceneBuildIndex) => await InternalLoadScene(sceneBuildIndex);

        static async UniTask InternalLoadScene(int sceneBuildIndex)
        {
            if (isLoading)
            {
                Debug.LogWarning("Could not load another scene while loading scene");
                return;
            }

            isLoading = true;
            isDone = false;

            try
            {
                try
                {
                    activeSceneChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                await UniTask.NextFrame();

                AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneBuildIndex);
                asyncOperation.allowSceneActivation = false;

                while (!asyncOperation.isDone || !asyncOperation.allowSceneActivation)
                {
                    //loadingAni.progress = asyncOperation.progress + 0.1f;
                    //asyncOperation.allowSceneActivation = loadingAni.isLongLoadingAniEnd && loadingAni.progress >= 1;

                    await UniTask.NextFrame(PlayerLoopTiming.Initialization);
                }
            }
            finally
            {
                isLoading = false;
                isDone = true;
            }
        }
    }
}
