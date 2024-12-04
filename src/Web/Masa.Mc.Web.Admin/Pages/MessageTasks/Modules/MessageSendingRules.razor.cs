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

    private bool _cronVisible;
    private string _tempCron = string.Empty;
    private string _nextRunTimeStr = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        GetNextRunTime();
    }

    private void OpenCronModal()
    {
        _cronVisible = true;

        if (CronExpression.IsValidExpression(Value.CronExpression))
        {
            _tempCron = Value.CronExpression;
        }
    }

    private async Task SetCronExpression()
    {
        if (CronExpression.IsValidExpression(_tempCron))
        {
            Value.CronExpression = _tempCron;
            GetNextRunTime();
            _cronVisible = false;
        }
        else
        {
            await PopupService.EnqueueSnackbarAsync(T("CronExpressionInvalid"), AlertTypes.Error);
        }

    }

    private void GetNextRunTime(int showCount = 10)
    {
        if (!CronExpression.IsValidExpression(Value.CronExpression))
        {
            _nextRunTimeStr = T("CronExpressionNotHasNextRunTime");
            return;
        }

        var sb = new StringBuilder();

        var startTime = DateTimeOffset.Now;

        var cronExpression = new CronExpression(Value.CronExpression);

        var timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(p => p.BaseUtcOffset == JsInitVariables.TimezoneOffset);

        if (timezone != null)
            cronExpression.TimeZone = timezone;

        for (int i = 0; i < showCount; i++)
        {
            var nextExcuteTime = cronExpression.GetNextValidTimeAfter(startTime);

            if (nextExcuteTime.HasValue)
            {
                startTime = nextExcuteTime.Value;
                sb.AppendLine(startTime.ToOffset(JsInitVariables.TimezoneOffset).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        if (sb.Length == 0)
        {
            _nextRunTimeStr = T("CronExpressionNotHasNextRunTime");
        }
        else
        {
            _nextRunTimeStr = sb.ToString();
        }
    }
}
