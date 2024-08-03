#nullable enable
namespace RuniEngine.Rhythms
{
    public interface IBeatValuePair<T> : IBeatValuePair
    {
        new T? value { get; set; }
    }
}
