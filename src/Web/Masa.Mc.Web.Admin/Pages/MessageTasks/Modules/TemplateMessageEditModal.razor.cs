// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private MessageTaskUpsertModel _model = new() { EntityType = MessageEntityTypes.Template, SystemId = MasaStackConsts.MC_SYSTEM_ID };
    private Guid _entityId;
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

    public async Task OpenModalAsync(MessageTaskDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<MessageTaskUpsertModel>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });

        _form?.ResetValidation();
    }

    private async Task GetFormDataAsync()
    {
        var dto = await MessageTaskService.GetAsync(_entityId) ?? new MessageTaskDto();
        _model = dto.Adapt<MessageTaskUpsertModel>();
        if (_model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection)
        {
            _selectReceivers = _model.Receivers;
        }
        else
        {
            _importReceivers = _model.Receivers;
        }
        _channelItems = _model.ChannelType.HasValue ? await ChannelService.GetListByTypeAsync(_model.ChannelType.Value) : new();

        if (_model.EntityId != default)
        {
            _messageInfo = await MessageTemplateService.GetAsync(_model.EntityId) ?? new();

            FillVariables(_model.Variables,_messageInfo.Items);
        }

        await HandleChannelChangeAsync();
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
        await MessageTaskService.UpdateAsync(_entityId, inputDto);
        Loading = false;
        if (_model.IsDraft)
        {
            await SuccessMessageAsync(T("MessageTaskEditMessage"));
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
        _model = new() { EntityType = MessageEntityTypes.Template, SystemId = MasaStackConsts.MC_SYSTEM_ID };
        _selectReceivers = new();
        _importReceivers = new();
        _selectReceiverType = false;
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private void HandleTemplateSelected(MessageTemplateDto item)
    {
        _messageInfo = item;
        if (item.Channel != null) _model.ChannelId = item.Channel.Id;
        _model.Sign = item.Sign;
        FillVariables(null, _messageInfo.Items);
        HandleChannelTypeChanged();
    }

    private void FillVariables(ExtraPropertyDictionary? variables, List<MessageTemplateItemDto> items)
    {
        if (variables == null)
            variables = new ExtraPropertyDictionary();

        foreach (var item in items)
        {
            if (!variables.Any(x => x.Key == item.Code))
            {
                variables.TryAdd(item.Code, string.Empty);
            }
        }
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
        if (_channelItems.Count > 0 && !_channelItems.Any(c => c.Id == _model.ChannelId))
        {
            _model.ChannelId = null;
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
