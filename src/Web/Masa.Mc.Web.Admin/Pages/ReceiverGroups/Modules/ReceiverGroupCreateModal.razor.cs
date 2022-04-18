namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverGroupCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    [Inject]
    public ReceiverGroupCaller ReceiverGroupCaller { get; set; } = default!;

    private ReceiverGroupCreateUpdateDto _model = new();
    private bool _visible;
    private string _userFilter;
    private bool _loading;
    private List<Guid> _userIds = new List<Guid>();
    private List<UserViewModel> _stateUserItems = new List<UserViewModel>()
    {
        new UserViewModel("xx团队","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com","团队"),
        new UserViewModel("xx角色","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com","角色"),
        new UserViewModel("xx组织架构","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com","组织架构"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel("鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
    };

    public async Task OpenModalAsync()
    {
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk(EditContext context)
    {
        if (!context.Validate())
        {
            return;
        }
        Loading = true;
        await ReceiverGroupCaller.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("ReceiverGroupCreateMessage"));
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
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    //private async Task QuerySelections(string v)
    //{
    //    if (string.IsNullOrEmpty(v) || v == _userFilter)
    //    {
    //        return;
    //    }

    //    _loading = true;
    //    await Task.Delay(500);
    //    _userItems = _stateUserItems.Where(e => e.DisplayName.Contains(v) || e.PhoneNumber.Contains(v) || e.Email.Contains(v)).ToList();
    //    _loading = false;
    //}

    public void Remove(UserViewModel item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }
}
