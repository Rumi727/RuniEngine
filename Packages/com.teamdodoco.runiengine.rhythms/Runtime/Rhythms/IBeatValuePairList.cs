namespace RuniEngine.Rhythms
{
    public interface IBeatValuePairList : ITypeList
    {
        void Add(double beat);
        void Sort();
    }
}
