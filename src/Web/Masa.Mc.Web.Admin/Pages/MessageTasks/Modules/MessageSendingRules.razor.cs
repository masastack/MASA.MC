namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageSendingRules : AdminCompontentBase
{
    [Parameter]
    public SendingRuleDto Value { get; set; } = new();

    [Parameter]
    public DateTime? SendTime { get; set; } = new();

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public EventCallback<DateTime?> SendTimeChanged { get; set; }

    private bool _datePickersShow;
    private bool _timePickersShow;
    public DateOnly? sendingDate;
    public TimeOnly? sendingTime;
    public bool _isTiming;

    protected override void OnParametersSet()
    {
        if (SendTime != null)
        {
            sendingDate = DateOnly.FromDateTime(SendTime.Value);
            sendingTime = TimeOnly.FromDateTime(SendTime.Value);
            _isTiming = true;
        }
        else
        {
            sendingDate = null;
            sendingTime = null;
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
        if (sendingDate == null)
        {
            return;
        }
        SendTime = _isTiming ? sendingDate.Value.ToDateTime(sendingTime ?? new TimeOnly(0, 0, 0)) : null;
        await SendTimeChanged.InvokeAsync(SendTime);
    }
}
