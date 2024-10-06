using System.Collections.Generic;

namespace RuniEngine.Rhythms
{
    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>>
    {
        public BeatValuePairList(T defaultValue) : base(defaultValue) { }
        public BeatValuePairList(T defaultValue, IEnumerable<BeatValuePair<T>> collection) : base(defaultValue, collection) { }
    }
}
