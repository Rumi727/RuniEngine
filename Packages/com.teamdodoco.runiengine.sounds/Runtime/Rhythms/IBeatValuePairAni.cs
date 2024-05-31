#nullable enable
using UnityEngine;

namespace RuniEngine.Rhythms
{
    public interface IBeatValuePairAni : IBeatValuePair
    {
        double length { get; set; }

        EasingFunction.Ease easingFunction { get; set; }
        AnimationCurve? curve { get; set; }
    }
}
