namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private ChannelCreateUpdateDto _model = new();
    private bool _visible;
    private List<ChannelType> channelTypeItems = Enum.GetValues(typeof(ChannelType))
        .Cast<ChannelType>().ToList();
    private ChannelExtraProperties _channelExtraPropertiesRef = default!;

    int _step = 1;

    ChannelService ChannelService => McCaller.ChannelService;

    public async Task OpenModalAsync()
    {
        _model.Type = ChannelType.Email;
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private void HandleSelectType(ChannelType Type)
    {
        _model.Type = Type;
        _step++;
    }

    private async Task HandleNextStepAsync()
    {
        if (_model.Type == default)
        {
            await WarningAsync(T("Description.Channel.Type.Required"));
            return;
        }
        _step++;
    }

    private async Task HandleOkAsync()
    {
        
        await _channelExtraPropertiesRef.UpdateExtraPropertiesAsync();
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await ChannelService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("ChannelCreateMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _step = 1;
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}
