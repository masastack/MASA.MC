// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private MessageTaskUpsertModel _model = new() { EntityType = MessageEntityTypes.Ordinary, SystemId = MasaStackProject.MC.Name };
    private Guid _entityId;
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();
    private bool _selectReceiverType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;
    MessageInfoService MessageInfoService => McCaller.MessageInfoService;

    protected override string? PageName { get; set; } = "MessageTaskBlock";

    private bool ComputedJumpUrlShow
    {
        get
        {
            return _model.ChannelType == ChannelTypes.WebsiteMessage || (_model.ChannelType == ChannelTypes.App && _model.ExtraProperties.IsWebsiteMessage) || (_model.ChannelType == ChannelTypes.WeixinWork && _model.MessageInfo.Type == (int)WeixinWorkTemplateTypes.TextCard);
        }
    }

    private bool ComputedTitleShow
    {
        get
        {
            return !(_model.ChannelType == ChannelTypes.Sms || (_model.ChannelType == ChannelTypes.WeixinWork && _model.MessageInfo.Type == (int)WeixinWorkTemplateTypes.Text));
        }
    }

    private bool ComputedJumpUrlRequired
    {
        get
        {
            return _model.ChannelType == ChannelTypes.WeixinWork && _model.MessageInfo.Type == (int)WeixinWorkTemplateTypes.TextCard;
        }
    }

    private bool ComputedMarkdown
    {
        get
        {
            return _model.ChannelType != ChannelTypes.App && _model.ChannelType != ChannelTypes.WeixinWork;
        }
    }

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
        var dto = await MessageTaskService.GetAsync(_entityId);
        if (dto != null)
        {
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
            var messageInfo = await MessageInfoService.GetAsync(_model.EntityId);
            if (messageInfo != null) _model.MessageInfo = messageInfo.Adapt<MessageInfoUpsertDto>();
        }
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
        _model = new() { EntityType = MessageEntityTypes.Ordinary, SystemId = MasaStackProject.MC.Name };
        _selectReceivers = new();
        _importReceivers = new();
        _selectReceiverType = false;
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
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

    private void HandleReceiverType(ReceiverTypes receiverType)
    {
        _model.ReceiverType = receiverType;
        _selectReceiverType = true;
        if (receiverType == ReceiverTypes.Broadcast)
        {
            _model.Step = 3;
        }
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

    private void HandleSelectTemplateType(int type)
    {
        _model.MessageInfo.IsJump = type != (int)WeixinWorkTemplateTypes.Text;
    }
}