using Mapster;

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    private ChannelCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;
    private List<ChannelType> channelTypeItems = Enum.GetValues(typeof(ChannelType))
        .Cast<ChannelType>().ToList();
    private ChannelExtraProperties _channelExtraPropertiesRef = default!;

    public async Task OpenModalAsync(ChannelDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<ChannelCreateUpdateDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await ChannelCaller.GetAsync(_entityId);
        _model = dto.Adapt<ChannelCreateUpdateDto>();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk(EditContext context)
    {
        await _channelExtraPropertiesRef.UpdateExtraPropertiesAsync();
        if (!context.Validate())
        {
            return;
        }
        Loading = true;
        await ChannelCaller.UpdateAsync(_entityId, _model);
        Loading = false;
        _visible = false;
        ResetForm();
        await SuccessMessageAsync(T("ChannelEditMessage"));
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDel()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"),async args =>
        {
            await DeleteAsync();
        }
        );
    }
    private async Task DeleteAsync()
    {
        Loading = true;
        await ChannelCaller.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("ChannelDeleteMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
    private void ResetForm()
    {
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}