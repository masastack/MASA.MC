using System.Collections.Generic;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.Contrib.StackSdks.Auth;

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;
public partial class SubjectAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> SelectedValues { get; set; } = new();

    [Parameter]
    public EventCallback<SubjectDto> OnAdd { get; set; }

    [Parameter]
    public EventCallback<FocusEventArgs> OnBlur { get; set; }

    public List<SubjectDto> Items { get; set; } = new();

    public string Search { get; set; } = "";

    [Inject]
    public IAuthClient AuthClient { get; set; } = default!;

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        if (Search == "")
        {
            Items.Clear();
        }
        else if (Search == search)
        {
            var subjects = await AuthClient.SubjectService.GetListAsync(search);
            var dtos = subjects.Adapt<List<SubjectDto>>();
            Items = dtos.Where(x => !SelectedValues.Contains(x.SubjectId)).ToList();
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
    }

    public void HandleOnSelectedItemUpdate(SubjectDto item)
    {
        Items.Remove(item);

        if (OnAdd.HasDelegate)
        {
            OnAdd.InvokeAsync(item);
        }
    }
}

