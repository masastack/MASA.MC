// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskSendModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form = default!;
    private SendMessageTaskInputDto _model = new();
    private MessageTaskDto _info = new();
    private Guid _entityId;
    private bool _visible;
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public async Task OpenModalAsync(MessageTaskDto model)
    {
        _entityId = model.Id;
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        _info = await MessageTaskService.GetAsync(_entityId) ?? new();
        _model = _info.Adapt<SendMessageTaskInputDto>();
        if (_model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection)
        {
            _selectReceivers = _model.Receivers;
        }
        else
        {
            _importReceivers = _model.Receivers;
        }
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTaskService.SendAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskSendMessage"));
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
        _info = new();
        _selectReceivers = new();
        _importReceivers = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}
