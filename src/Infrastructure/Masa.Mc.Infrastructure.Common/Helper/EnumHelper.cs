namespace Masa.Mc.Infrastructure.Common.Helper;

public class EnumHelper
{
    public static List<TEnum> GetEnumList<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
    }
}
