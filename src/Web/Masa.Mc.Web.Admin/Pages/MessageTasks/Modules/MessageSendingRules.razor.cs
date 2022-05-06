// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageSendingRules : AdminCompontentBase
{
    [Parameter]
    public SendRuleDto Value { get; set; } = new();

    [Parameter]
    public DateTime? SendTime { get; set; } = new();

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public EventCallback<DateTime?> SendTimeChanged { get; set; }

    private bool _datePickersShow;
    private bool _timePickersShow;
    private DateOnly? _sendingDate;
    private TimeOnly? _sendingTime;
    private bool _isTiming;

    protected override void OnParametersSet()
    {
        if (SendTime != null)
        {
            _sendingDate = DateOnly.FromDateTime(SendTime.Value);
            _sendingTime = TimeOnly.FromDateTime(SendTime.Value);
            _isTiming = true;
        }
        else
        {
            _sendingDate = null;
            _sendingTime = null;
        }
    }

    private async void HandleDatePickerInputAsync()
    {
        _datePickersShow = false;
        await FillSendTimeAsync();
    }

    private async Task HandleTimePickerChangeAsync()
    {
        await FillSendTimeAsync();
    }

    private async Task FillSendTimeAsync()
    {
        if (_sendingDate == null)
        {
            return;
        }
        SendTime = _isTiming ? _sendingDate.Value.ToDateTime(_sendingTime ?? new TimeOnly(0, 0, 0)) : null;
        await SendTimeChanged.InvokeAsync(SendTime);
    }
}
