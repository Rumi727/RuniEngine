using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListColor : BeatValuePairAniList<JColor>
    {
        public BeatValuePairAniListColor(JColor defaultValue) : base(defaultValue) { }

        public override JColor ValueCalculate(double t, EasingFunction.Ease easingFunction, JColor previousValue, JColor value)
        {
            float r = (float)EasingFunction.EasingCalculate(previousValue.r, value.r, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(previousValue.g, value.g, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(previousValue.b, value.b, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(previousValue.a, value.a, t, easingFunction);

            return new JColor(r, g, b, a);
        }
    }
}
