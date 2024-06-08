#nullable enable
using RuniEngine.Booting;
using RuniEngine.Splashs.UI;
using UnityEngine;

namespace RuniEngine.Splashs
{
    public sealed class DefaultSplashAni : MonoBehaviour
    {
        [SerializeField, NotNullField] Animator? animator;
        [SerializeField, NotNullField] SplashScreenProgressBar? progressBar;

        [SerializeField] int layer = 0;
        [SerializeField] string startParameter = "Start";
        [SerializeField] string endParameter = "End";

        int startHash;
        int endHash;
        void OnEnable()
        {
            startHash = Animator.StringToHash(startParameter);
            endHash = Animator.StringToHash(endParameter);
        }

        void Update()
        {
            if (animator == null)
                return;

            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layer);

            bool start = animator.GetBool(startHash);
            bool end = animator.GetBool(endHash);

            if (SplashScreen.isPlaying)
            {
                if (start && currentState.normalizedTime >= 1 && BootLoader.isAllLoaded)
                {
                    SplashScreen.resourceLoadable = true;

                    animator.SetBool(startHash, false);
                    animator.SetBool(endHash, true);
                }
                else if (start && currentState.normalizedTime >= 0.4370370370f)
                    SplashScreen.resourceLoadable = true;
                else if (end && currentState.normalizedTime >= 1)
                {
                    SplashScreen.isPlaying = false;

                    animator.SetBool(startHash, false);
                    animator.SetBool(endHash, false);
                }
                else if (!start && !end)
                    animator.SetBool(startHash, true);
            }
            else if (start || end)
            {
                animator.SetBool(startHash, false);
                animator.SetBool(endHash, false);
            }

            if (progressBar != null)
            {
                if (start && currentState.normalizedTime < 0.4370370370f)
                {
                    progressBar.progress = 0;
                    progressBar.allowNoResponseAni = false;
                }
                else
                {
                    progressBar.allowNoResponseAni = true;

                    if (BootLoader.resourceTask != null)
                    {
                        float value = BootLoader.resourceTask.progress / BootLoader.resourceTask.maxProgress;
                        if (float.IsNormal(value))
                            progressBar.progress = value;
                    }
                    else if (BootLoader.isAllLoaded)
                        progressBar.progress = 1;
                    else
                        progressBar.progress = 0;
                }
            }
        }
    }
}
