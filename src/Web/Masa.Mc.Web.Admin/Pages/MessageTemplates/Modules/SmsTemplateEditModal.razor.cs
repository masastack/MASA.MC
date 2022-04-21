namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class SmsTemplateEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTemplateCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private List<SmsTemplateDto> _templateItems = new();
    private ChannelType _channelType;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    SmsTemplateService SmsTemplateService => McCaller.SmsTemplateService;

    public async Task OpenModalAsync(MessageTemplateDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<MessageTemplateCreateUpdateDto>();
        _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
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
        await HandleSelectChannelTypeAsync(_channelType);
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
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
        await ConfirmAsync(T("DeletionConfirmationMessage"), async args => { await DeleteAsync(); });
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

    private async Task HandleSelectChannelTypeAsync(ChannelType Type)
    {
        _channelItems = await ChannelService.GetListByTypeAsync(Type);
    }

    private void HandleTemplateSelected(SmsTemplateDto smsTemplate)
    {
        _model.DisplayName = smsTemplate.TemplateName;
        _model.Content = smsTemplate.TemplateContent;
        _model.AuditStatus = smsTemplate.AuditStatus;
        _model.AuditReason = smsTemplate.AuditReason;
        _model.TemplateType = (int)smsTemplate.TemplateType;
        _model.Items = ParseTemplateItem(smsTemplate.TemplateContent);
    }

    private async Task HandleChannelChangeAsync()
    {
        _model.DisplayName = string.Empty;
        _model.Content = string.Empty;
        _model.TemplateId = string.Empty;
        _model.Items = new();

        _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
    }

    private List<MessageTemplateItemDto> ParseTemplateItem(string content)
    {
        string startstr = "\\${";
        string endstr = "}";
        var paramList = UtilHelper.MidStrEx(content, startstr, endstr);
        return paramList.Select(x => new MessageTemplateItemDto { Code = x, MappingCode = x }).ToList();
    }

    private async Task SynchroAsync()
    {
        Loading = true;
        await SmsTemplateService.SynchroAsync(new SmsTemplateSynchroInput(_model.ChannelId));
        _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
        Loading = false;
    }
}
