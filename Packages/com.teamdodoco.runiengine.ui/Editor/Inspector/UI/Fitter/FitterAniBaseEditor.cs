using RuniEngine.UI.Fitter;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    public abstract class FitterAniBaseEditor<TTarget> : UIAniBaseEditor<TTarget> where TTarget : Object, IFitterAni
    {

    }
}
