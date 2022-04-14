namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class WebsiteMessageTemplateCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    [Inject]
    public MessageTemplateCaller MessageTemplateCaller { get; set; } = default!;

    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    private MForm _form;
    private MessageTemplateCreateUpdateDto _model = new();
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelType _channelType;

    public async Task OpenModalAsync(ChannelType? channelType)
    {
        if (channelType.HasValue)
        {
            _channelType = channelType.Value;
            await HandleSelectChannelTypeAsync(_channelType);
        }
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

    private async Task HandleOkAsync()
    {
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTemplateCaller.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTemplateCreateMessage"));
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
        _channelItems = await ChannelCaller.GetListByTypeAsync(Type);
    }
}
