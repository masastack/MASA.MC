// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private MessageTaskUpsertModel _model = new() { EntityType = MessageEntityTypes.Template, SystemId = MasaStackProject.MC.Name };
    private bool _visible;
    private List<MessageTemplateDto> _templateItems = new();
    private MessageTemplateDto _messageInfo = new();
    private List<ChannelDto> _channelItems = new();
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();
    private bool _selectReceiverType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;
    ChannelService ChannelService => McCaller.ChannelService;

    public async Task OpenModalAsync()
    {
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });

        _form?.ResetValidation();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleSaveAsync()
    {
        _model.IsDraft = true;
        await HandleOkAsync();
    }

    private async Task HandleSendAsync()
    {
        _model.IsDraft = false;
        await HandleOkAsync();
    }

    private async Task HandleOkAsync()
    {
        Check.NotNull(_form, "form not found");

        SetReceivers();
        if (!_form.Validate())
        {
            return;
        }
        Loading = true;
        var inputDto = _model.Adapt<MessageTaskUpsertDto>();
        await MessageTaskService.CreateAsync(inputDto);
        Loading = false;
        if (_model.IsDraft)
        {
            await SuccessMessageAsync(T("MessageTaskCreateMessage"));
        }
        else
        {
            await SuccessMessageAsync(T("MessageTaskSendMessage"));
        }
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new() { EntityType = MessageEntityTypes.Template, SystemId = MasaStackProject.MC.Name };
        _selectReceivers = new();
        _importReceivers = new();
        _selectReceiverType = false;
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
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
            _model.Step = 3;
        }
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = _model.ChannelType.HasValue ? await ChannelService.GetListByTypeAsync(_model.ChannelType.Value) : new();
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
            _model.Step = 1;
        }

        if (_selectReceiverType)
        {
            _selectReceiverType = false;
        }
    }

    private void HandleSendingRuleBack()
    {
        _model.Step = 2;
        if (_model.ReceiverType == ReceiverTypes.Broadcast)
        {
            _selectReceiverType = false;
        }
    }

    private void HandleNextStep()
    {
        SetReceivers();
        _model.IsDraft = false;
        if (!_form.Validate())
        {
            return;
        }
        _model.Step++;
    }

    private void SetReceivers()
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
    }
}
