namespace Masa.Mc.Contracts.Admin.Dtos;

public class ItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public ItemDto()
    {

    }

    public ItemDto(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
