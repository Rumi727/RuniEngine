#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Fitter
{
    public abstract class FitterAniBase : UIAniBase, IFitterAni
    {
        protected DrivenRectTransformTracker tracker;

        /// <summary>
        /// Please put <see cref="OnDisable"/> when overriding
        /// </summary>
        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }
    }
}
