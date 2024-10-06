using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListColor32 : BeatValuePairAniList<JColor32>
    {
        public BeatValuePairAniListColor32(JColor32 defaultValue) : base(defaultValue) { }

        public override JColor32 ValueCalculate(double t, EasingFunction.Ease easingFunction, JColor32 previousValue, JColor32 value)
        {
            byte r = (byte)EasingFunction.EasingCalculate(previousValue.r, value.r, t, easingFunction);
            byte g = (byte)EasingFunction.EasingCalculate(previousValue.g, value.g, t, easingFunction);
            byte b = (byte)EasingFunction.EasingCalculate(previousValue.b, value.b, t, easingFunction);
            byte a = (byte)EasingFunction.EasingCalculate(previousValue.a, value.a, t, easingFunction);

            return new JColor32(r, g, b, a);
        }
    }
}
