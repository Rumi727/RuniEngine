#nullable enable
using RuniEngine.UI;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.UI
{
    public abstract class UIBaseEditor<TTarget> : CustomInspectorBase<TTarget> where TTarget : Object, IUI
    {

    }
}
