namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

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

    private ChannelEmailingExtraProperties _emailingExtraPropertiesRef = default!;
    private ChannelSmsExtraProperties _smsExtraPropertiesRef = default!;
    private ChannelSiteExtraProperties _siteExtraPropertiesRef = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Watcher
            .Watch<ExtraPropertyDictionary>(nameof(Value), async val =>
             {
                 await ValueChanged.InvokeAsync(Value);
             });
    }

    public void HandleChangeAsync(string value, string key)
    {
        Value[key] = value;
    }

    public async Task UpdateExtraPropertiesAsync()
    {
        if (Type == ChannelType.Emailing) await _emailingExtraPropertiesRef.HandleChangeAsync();
        if (Type == ChannelType.Sms) await _smsExtraPropertiesRef.HandleChangeAsync();
        if (Type == ChannelType.Site) await _siteExtraPropertiesRef.HandleChangeAsync();
        await ValueChanged.InvokeAsync(Value);
    }

}
