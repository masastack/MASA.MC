namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;
public partial class SubjectAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<SubjectDto> OnRemove { get; set; }

    [Parameter]
    public EventCallback<SubjectDto> OnAdd { get; set; }

    [Parameter]
    public List<Guid> SelectedValues { get; set; } = new();

    [Parameter]
    public EventCallback<FocusEventArgs> OnBlur { get; set; }

    public List<SubjectDto> Items { get; set; } = new();

    public List<SubjectDto> SubjectSelect { get; set; } = new();

    public string Search { get; set; } = "";

    SubjectService SubjectService => McCaller.SubjectService;

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        if (Search == "")
        {
            Items.Clear();
        }
        else if (Search == search)
        {
            Items = await SubjectService.GetListAsync(new GetSubjectInputDto(search));
        }
    }

    public string TextView(SubjectDto subject)
    {
        if (!string.IsNullOrEmpty(subject.Name)) return subject.Name;
        if (!string.IsNullOrEmpty(subject.DisplayName)) return subject.DisplayName;
        if (!string.IsNullOrEmpty(subject.PhoneNumber)) return subject.PhoneNumber;
        if (!string.IsNullOrEmpty(subject.Email)) return subject.Email;
        return "";
    }

    public void ResetForm()
    {
        Items = new();
        ResetValue();
    }

    public void ResetValue()
    {
        Value = new();
        if (ValueChanged.HasDelegate)
        {
            ValueChanged.InvokeAsync(Value);
        }
    }

    public void Remove(SubjectDto item)
    {
        var index = Value.IndexOf(item.SubjectId);
        if (index >= 0)
        {
            Value.RemoveAt(index);
        }

        if (ValueChanged.HasDelegate)
        {
            ValueChanged.InvokeAsync(Value);
        }

        if (OnRemove.HasDelegate)
        {
            OnRemove.InvokeAsync(item);
        }
    }

    public void HandleOnSelectedItemUpdate(SubjectDto item)
    {
        if (SelectedValues.Contains(item.SubjectId))
        {
            Remove(item);
        }
        else
        {
            if (OnAdd.HasDelegate)
            {
                OnAdd.InvokeAsync(item);
            }
        }
    }
}

