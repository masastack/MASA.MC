namespace Masa.Mc.Web.Admin.Components.Modules.MessageTasks;
public partial class MessageTaskReceiverAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public MessageTaskReceiverTypes? Type { get; set; }

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    public List<MessageTaskReceiverDto> Items { get; set; } = new();

    public List<MessageTaskReceiverDto> SubjectSelect { get; set; } = new();

    public string Search { get; set; } = "";

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        await Task.Delay(300);
        if (Search == "")
        {
            SubjectSelect.Clear();
        }
        else if (Search == search)
        {
            var subjectList = await MessageTaskService.GetMessageTaskReceiverListAsync(search);
            if (Type.HasValue)
            {
                subjectList = subjectList.Where(x => x.Type == Type).ToList();
            }
            SubjectSelect = subjectList;
        }
    }

    public string TextView(MessageTaskReceiverDto subject)
    {
        if (string.IsNullOrEmpty(subject.DisplayName) is false) return subject.DisplayName;
        if (string.IsNullOrEmpty(subject.PhoneNumber) is false) return subject.PhoneNumber;
        if (string.IsNullOrEmpty(subject.Email) is false) return subject.Email;
        return "";
    }

    private async Task HandleValueChanged(List<Guid> value)
    {
        value = value ?? new();
        var list = SubjectSelect.Where(x => value.Contains(x.SubjectId)).ToList();
        Items = list;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
    }
}

