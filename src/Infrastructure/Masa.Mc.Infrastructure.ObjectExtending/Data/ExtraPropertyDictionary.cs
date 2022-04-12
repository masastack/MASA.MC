namespace Masa.Mc.Infrastructure.ObjectExtending;

[Serializable]
public class ExtraPropertyDictionary : Dictionary<string, object>
{
    public ExtraPropertyDictionary()
    {

    }

    public ExtraPropertyDictionary(IDictionary<string, object> dictionary)
        : base(dictionary)
    {
    }
}