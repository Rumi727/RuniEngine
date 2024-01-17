#nullable enable
using RuniEngine.Booting;
using UnityEngine;

namespace RuniEngine.Splashs
{
    public sealed class DefaultSplashAni : MonoBehaviour
    {
        [SerializeField, NotNullField] Animator? animator;

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
                if (start && currentState.normalizedTime >= 1 && BootLoader.allLoaded)
                {
                    animator.SetBool(startHash, false);
                    animator.SetBool(endHash, true);
                }
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
        }
    }
}
