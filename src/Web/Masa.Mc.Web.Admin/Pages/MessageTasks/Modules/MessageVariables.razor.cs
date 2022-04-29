namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageVariables : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }


    private List<ItemDto> _items = new();

    protected override void OnParametersSet()
    {
        _items = Value.Select(x => new ItemDto(x.Key, x.Value?.ToString() ?? string.Empty)).ToList();
    }

    public async Task HandleChangeAsync()
    {
        var extraProperty = new ExtraPropertyDictionary();
        foreach (var item in _items)
        {
            extraProperty.TryAdd(item.Name, item.Value);
        }
        Value = extraProperty;
        await ValueChanged.InvokeAsync(Value);
    }
}
