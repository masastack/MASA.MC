﻿namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskCreateUpdateDto _model = new() { ReceiverType = ReceiverType.Assign, EntityType = MessageEntityType.Ordinary };
    private Guid _entityId;
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelType _channelType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;
    MessageInfoService MessageInfoService => McCaller.MessageInfoService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
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
        if (dto != null)
        {
             _channelType = dto.Channel.Type;
            _model = dto.Adapt<MessageTaskCreateUpdateDto>();
            _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
            var messageInfo = await MessageInfoService.GetAsync(_model.EntityId);
            if (messageInfo != null) _model.MessageInfo = messageInfo.Adapt<MessageInfoCreateUpdateDto>();
        }
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
        await SuccessMessageAsync(T("MessageTaskCreateMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new() { ReceiverType = ReceiverType.Assign, EntityType = MessageEntityType.Ordinary };
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
        if (_channelType != ChannelType.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverType.Assign;
        }
    }
}