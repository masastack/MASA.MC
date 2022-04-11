namespace MASA.MC.Infrastructure.ObjectExtending;

[Serializable]
public class ExtraPropertyDictionary : ConcurrentDictionary<string, object>
{
    public ExtraPropertyDictionary()
    {

    }

    public ExtraPropertyDictionary(IDictionary<string, object> dictionary)
        : base(dictionary)
    {
    }
}