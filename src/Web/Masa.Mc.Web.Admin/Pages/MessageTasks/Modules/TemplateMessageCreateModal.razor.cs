// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskUpsertDto _model = new() { EntityType = MessageEntityTypes.Template };
    private bool _visible;
    private List<MessageTemplateDto> _templateItems = new();
    private MessageTemplateDto _messageInfo = new();
    private List<ChannelDto> _channelItems = new();
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();
    private List<string> _tabs = new();
    private string _tab = "";
    private bool _selectReceiverType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;
    ChannelService ChannelService => McCaller.ChannelService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _tabs = new List<string> { T("DisplayName.MessageInfoContent"), T("DisplayName.MessageTaskReceiver"), T("DisplayName.MessageTaskSendingRule") };
        _tab = _tabs[0];
    }

    public async Task OpenModalAsync()
    {
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task HandleCancel()
    {
        _visible = false;
        await ResetForm();
    }

    private async Task HandleOkAsync(bool isDraft)
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
        _model.IsDraft = isDraft;
        _model.ChannelType = _messageInfo.Channel.Type;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTaskService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskCreateMessage"));
        _visible = false;
        await ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task ResetForm()
    {
        _model = new() { EntityType = MessageEntityTypes.Template };
        _selectReceivers = new();
        _importReceivers = new();
        await _form.ResetValidationAsync();
    }

    private async Task HandleVisibleChanged(bool val)
    {
        if (!val) await HandleCancel();
    }

    private async Task HandleTemplateSelectedAsync(MessageTemplateDto item)
    {
        if (item.Channel != null) _channelItems = await ChannelService.GetListByTypeAsync(item.Channel.Type);
        _messageInfo = item;
        if (item.Channel != null) _model.ChannelId = item.Channel.Id;
        _model.Sign = item.Sign;
        _model.Variables = FillVariables(_messageInfo.Items);
        HandleChannelTypeChanged();
    }

    private ExtraPropertyDictionary FillVariables(List<MessageTemplateItemDto> items)
    {
        var source = new ExtraPropertyDictionary();
        foreach (var item in items)
        {
            source.TryAdd(item.Code, string.Empty);
        }
        return source;
    }

    private void HandleChannelTypeChanged()
    {
        if (_messageInfo.Channel?.Type != ChannelTypes.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverTypes.Assign;
        }
        else
        {
            _model.ReceiverType = default;
        }
    }

    private void HandleReceiverType(ReceiverTypes receiverType)
    {
        _model.ReceiverType = receiverType;
        _selectReceiverType = true;
        if (receiverType == ReceiverTypes.Broadcast)
        {
            _tab = _tabs[2];
        }
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(_model.ChannelType);
        if (_model.ChannelType != ChannelTypes.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverTypes.Assign;
        }
        else
        {
            _model.ReceiverType = default;
        }
    }

    private async Task HandleChannelChangeAsync()
    {
        var inputDto = new GetMessageTemplateInputDto(999) { AuditStatus = MessageTemplateAuditStatuses.Adopt, ChannelId = _model.ChannelId };
        _templateItems = (await MessageTemplateService.GetListAsync(inputDto)).Result;
    }

    private void HandleReceiverBack()
    {
        if (_model.ChannelType != ChannelTypes.WebsiteMessage || !_selectReceiverType)
        {
            _tab = _tabs[0];
        }

        if (_selectReceiverType)
        {
            _selectReceiverType = false;
        }
    }

    private void HandleSendingRuleBack()
    {
        _tab = _tabs[1];
        if (_model.ReceiverType == ReceiverTypes.Broadcast)
        {
            _selectReceiverType = false;
        }
    }
}
