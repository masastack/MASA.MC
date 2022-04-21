namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class WebsiteMessageTemplateEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTemplateCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelType _channelType;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    public async Task OpenModalAsync(MessageTemplateDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<MessageTemplateCreateUpdateDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await MessageTemplateService.GetAsync(_entityId);
        _model = dto.Adapt<MessageTemplateCreateUpdateDto>();
        _channelType = dto.Channel.Type;
        await HandleSelectChannelType(_channelType);
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk()
    {
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTemplateService.UpdateAsync(_entityId, _model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTemplateEditMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDelAsync()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), DeleteAsync);
    }

    private async Task DeleteAsync()
    {
        Loading = true;
        await MessageTemplateService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("MessageTemplateDeleteMessage"));
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

    private async Task HandleSelectChannelType(ChannelType Type)
    {
        _channelItems = await ChannelService.GetListByTypeAsync(Type);
    }
}
