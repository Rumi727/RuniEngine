using UnityEngine;

namespace RuniEngine.UI.Fitter
{
    public abstract class FitterAniBase : UIAniBase, IFitterAni
    {
        protected DrivenRectTransformTracker tracker;

        /// <summary>
        /// Please put <see cref="OnDestroy"/> when overriding
        /// </summary>
        protected override void OnDestroy()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }
    }
}
