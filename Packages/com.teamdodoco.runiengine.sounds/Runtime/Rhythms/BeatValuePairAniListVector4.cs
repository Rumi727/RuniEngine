#nullable enable
using RuniEngine.Jsons;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListVector4 : BeatValuePairAniList<JVector4>
    {
        public BeatValuePairAniListVector4(JVector4 defaultValue) : base(defaultValue) { }

        public override JVector4 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector4 previousValue, JVector4 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(previousValue.z, value.z, t, easingFunction);
            float w = (float)EasingFunction.EasingCalculate(previousValue.w, value.w, t, easingFunction);

            return new JVector4(x, y, z, w);
        }
    }
}
