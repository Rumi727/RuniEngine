#nullable enable
using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public BeatValuePairAniListVector2(JVector2 defaultValue) : base(defaultValue) { }

        public override JVector2 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector2 previousValue, JVector2 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);

            return new JVector2(x, y);
        }
    }
}
