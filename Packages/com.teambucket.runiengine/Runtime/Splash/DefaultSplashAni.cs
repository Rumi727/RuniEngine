#nullable enable
using RuniEngine.Booting;
using RuniEngine.Logo;
using UnityEngine;

namespace RuniEngine.Splash
{
    public sealed class DefaultSplashAni : MonoBehaviour
    {
        [SerializeField, NotNullField] MainLogo? logo;
        [SerializeField] float pauseProgress = 2.7f;
        [SerializeField] float endProgress = 2.7f;

        void Update()
        {
            if (logo == null)
                return;

            SplashScreen.isEnd = false;

            if (SplashScreen.isPlaying)
            {
                if (logo.aniProgress >= endProgress)
                    SplashScreen.isEnd = true;
                else if (logo.aniProgress >= pauseProgress)
                {
                    if (BootLoader.allLoaded)
                        logo.state = MainLogoState.start;
                    else
                    {
                        logo.state = MainLogoState.idle;
                        logo.aniProgress = pauseProgress;
                    }
                }
                else
                    logo.state = MainLogoState.start;
            }
            else
            {
                logo.state = MainLogoState.idle;
                logo.aniProgress = 0;
            }
        }
    }
}
