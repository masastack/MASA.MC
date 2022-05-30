// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Pickers;

public partial class DateRangePicker
{
    [Parameter]
    public DateTimeOffset? StartTime { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset?> StartTimeChanged { get; set; }

    [Parameter]
    public DateTimeOffset? EndTime { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset?> EndTimeChanged { get; set; }

    [Parameter]
    public EventCallback OnChange{ get; set; }

    private bool StartTimeVisible { get; set; }

    private bool EndTimeVisible { get; set; }

    private DateOnly? _StartTime { get; set; }

    private DateOnly? _EndTime { get; set; }

    protected override void OnParametersSet()
    {
        _StartTime = StartTime.HasValue ? DateOnly.FromDateTime(StartTime.Value.DateTime) : null;
        _EndTime = EndTime.HasValue ? DateOnly.FromDateTime(EndTime.Value.DateTime) : null;
    }

    private async Task UpdateStartTimeAsync(DateOnly? dateTime)
    {
        var startTime = dateTime?.ToDateTime(TimeOnly.MinValue);
        if (startTime > EndTime) await WarningAsync(T("Start time cannot be greater than end time"));
        else
        {
            StartTime = startTime;
            if (StartTimeChanged.HasDelegate) await StartTimeChanged.InvokeAsync(StartTime);
            if (OnChange.HasDelegate) await OnChange.InvokeAsync();
        }
    }

    private async Task UpdateEndTimeAsync(DateOnly? dateTime)
    {
        var endTime = dateTime?.ToDateTime(TimeOnly.MaxValue);
        if (endTime < StartTime) await WarningAsync(T("End time cannot be less than start time"));
        else
        {
            EndTime = endTime;
            if (StartTimeChanged.HasDelegate) await EndTimeChanged.InvokeAsync(EndTime);
            if (OnChange.HasDelegate) await OnChange.InvokeAsync();
        }
    }
}
