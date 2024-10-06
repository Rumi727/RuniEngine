using System.Collections.Generic;

namespace RuniEngine.Rhythms
{
    public abstract class BeatValuePairAniList<T> : BeatValuePairAniList<T, BeatValuePairAni<T>>
    {
        public BeatValuePairAniList(T defaultValue) : base(defaultValue) { }
        public BeatValuePairAniList(T defaultValue, IEnumerable<BeatValuePairAni<T>> collection) : base(defaultValue, collection) { }
    }
}
