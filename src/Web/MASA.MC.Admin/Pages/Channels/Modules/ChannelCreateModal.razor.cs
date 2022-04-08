namespace MASA.MC.Admin.Pages.Channels.Modules;

public partial class ChannelCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    private ChannelCreateUpdateDto _model = new();
    private MForm _form;
    private bool _visible;
    private List<ChannelType> channelTypeItems = Enum.GetValues(typeof(ChannelType))
        .Cast<ChannelType>().ToList();

    int _step = 1;

    public async Task OpenModalAsync()
    {
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
        if (_model.Type==default)
        {
            await WarningAsync(T("Please select the channel type first"));
            return;
        }
        _step++;
    }

    private async Task HandleOk(EditContext context)
    {
        if (!await _form.ValidateAsync())
        {
            return;
        }
        await ChannelCaller.CreateAsync(_model);
        await SuccessMessageAsync(T("Create channel data success"));
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
}
