#nullable enable
using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListVector3 : BeatValuePairAniList<JVector3>
    {
        public BeatValuePairAniListVector3(JVector3 defaultValue) : base(defaultValue) { }

        public override JVector3 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector3 previousValue, JVector3 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(previousValue.z, value.z, t, easingFunction);

            return new JVector3(x, y, z);
        }
    }
}
