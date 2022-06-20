using Masa.Mc.Contracts.Admin.Dtos.Subjects;

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;
public partial class SubjectAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<SubjectDto> OnSelectedItemUpdate { get; set; }

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

    private async Task HandleValueChanged(List<Guid> value)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        var list = Items.Where(x => value.Contains(x.SubjectId)).ToList();
        SubjectSelect = list;
    }
}

