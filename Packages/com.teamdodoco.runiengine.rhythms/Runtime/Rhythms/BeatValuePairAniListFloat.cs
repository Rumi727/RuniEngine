#nullable enable
namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public BeatValuePairAniListFloat(float defaultValue) : base(defaultValue) { }

        public override float ValueCalculate(double t, EasingFunction.Ease easingFunction, float previousValue, float value) =>
            (float)EasingFunction.EasingCalculate(previousValue, value, t, easingFunction);
    }
}
