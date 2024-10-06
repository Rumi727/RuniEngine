namespace RuniEngine.Rhythms
{
    public class BeatValuePairAniListInt : BeatValuePairAniList<int>
    {
        public BeatValuePairAniListInt(int defaultValue) : base(defaultValue) { }

        public override int ValueCalculate(double t, EasingFunction.Ease easingFunction, int previousValue, int value) =>
            EasingFunction.EasingCalculate(previousValue, value, t, easingFunction).RoundToInt();
    }
}
