using RuniEngine.UI.Fitter;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI.Fitter
{
    public abstract class FitterBaseEditor<TTarget> : UIBaseEditor<TTarget> where TTarget : Object, IFitter
    {

    }
}
