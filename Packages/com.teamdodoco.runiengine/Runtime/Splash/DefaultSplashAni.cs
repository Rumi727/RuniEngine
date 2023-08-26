#nullable enable
using RuniEngine.Booting;
using RuniEngine.Logo;
using UnityEngine;

namespace RuniEngine.Splash
{
    public sealed class DefaultSplashAni : MonoBehaviour
    {
        [SerializeField, NotNullField] Animator? logo;

        [SerializeField] int mainLayer = 0;
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
            if (logo == null)
                return;

            AnimatorStateInfo currentState = logo.GetCurrentAnimatorStateInfo(mainLayer);

            bool start = logo.GetBool(startHash);
            bool end = logo.GetBool(endHash);

            if (SplashScreen.isPlaying)
            {
                if (start && currentState.normalizedTime >= 1 && BootLoader.allLoaded)
                {
                    logo.SetBool(startHash, false);
                    logo.SetBool(endHash, true);
                }
                else if (end && currentState.normalizedTime >= 1)
                {
                    SplashScreen.isPlaying = false;

                    logo.SetBool(startHash, false);
                    logo.SetBool(endHash, false);
                }
                else if (!start && !end)
                    logo.SetBool(startHash, true);
            }
            else if (start || end)
            {
                logo.SetBool(startHash, false);
                logo.SetBool(endHash, false);
            }
        }
    }
}
