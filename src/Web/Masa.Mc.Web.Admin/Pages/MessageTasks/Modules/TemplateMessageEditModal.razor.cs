namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;
    private List<MessageTemplateDto> _templateItems = new();
    private MessageTemplateDto _messageInfo = new();

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;
    ChannelService ChannelService => McCaller.ChannelService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var input = new GetMessageTemplateInput();
        _templateItems = (await MessageTemplateService.GetListAsync(input)).Result;
    }

    public async Task OpenModalAsync(MessageTaskDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<MessageTaskCreateUpdateDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await MessageTaskService.GetAsync(_entityId);
        _model = dto.Adapt<MessageTaskCreateUpdateDto>();
        _messageInfo = await MessageTemplateService.GetAsync(_model.EntityId) ?? new();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync(bool IsEnabled)
    {
        _model.IsEnabled = IsEnabled;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTaskService.UpdateAsync(_entityId, _model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskEditMessage"));
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
