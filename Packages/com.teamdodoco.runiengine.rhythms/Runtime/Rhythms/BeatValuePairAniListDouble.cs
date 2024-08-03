#nullable enable
namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public BeatValuePairAniListDouble(double defaultValue) : base(defaultValue) { }

        public override double ValueCalculate(double t, EasingFunction.Ease easingFunction, double previousValue, double value) =>
            EasingFunction.EasingCalculate(previousValue, value, t, easingFunction);
    }
}
