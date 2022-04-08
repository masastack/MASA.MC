namespace MASA.MC.Admin.Pages.Channels.Modules;

public partial class ChannelExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ChannelType Type { get; set; }

    [Parameter]
    public ExtraPropertyDictionary Value
    {
        get
        {
            return GetValue<ExtraPropertyDictionary>();
        }
        set
        {
            SetValue(value);
        }
    }

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Watcher
            .Watch<ExtraPropertyDictionary>(nameof(Value), val =>
            {
                ValueChanged.InvokeAsync(Value);
            });
    }

    public void HandleChangeAsync(string value,string key)
    {
        Value[key] = value;
    }
}
