// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageSendingRules : AdminCompontentBase
{
    [Parameter]
    public SendRuleDto Value { get; set; } = new();

    [Parameter]
    public EventCallback<SendRuleDto?> ValueChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private bool _datePickersShow;
    private bool _timePickersShow;
    private DateOnly? _sendingDate;
    private TimeOnly? _sendingTime;

    protected override void OnParametersSet()
    {
        if (Value.SendTime != null)
        {
            _sendingDate = DateOnly.FromDateTime(Value.SendTime.Value);
            _sendingTime = TimeOnly.FromDateTime(Value.SendTime.Value);
            Value.IsTiming = true;
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
        Value.SendTime = Value.IsTiming ? _sendingDate.Value.ToDateTime(_sendingTime ?? new TimeOnly(0, 0, 0)) : null;
        await ValueChanged.InvokeAsync(Value);
    }
}
