using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListRect : BeatValuePairAniList<JRect>
    {
        public BeatValuePairAniListRect(JRect defaultValue) : base(defaultValue) { }

        public override JRect ValueCalculate(double t, EasingFunction.Ease easingFunction, JRect previousValue, JRect value)
        {
            float r = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(previousValue.width, value.width, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(previousValue.height, value.height, t, easingFunction);

            return new JRect(r, g, b, a);
        }
    }
}
