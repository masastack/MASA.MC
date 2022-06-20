namespace Masa.Mc.Web.Admin.Layouts;

public partial class McLayout
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter, EditorRequired]
    public string? DefaultRoute { get; set; }

    [Parameter, EditorRequired]
    public string? Logo { get; set; }

    [Parameter, EditorRequired]
    public string? MiniLogo { get; set; }

    [Parameter, EditorRequired]
    public List<Nav>? NavItems { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        NavItems ??= new List<Nav>();
        FlattenedNavs = FlattenNavs(NavItems, true);
    }

    private List<Nav> FlattenedNavs { get; set; } = new();

    private List<Nav> FlattenNavs(List<Nav> tree, bool excludeNavHasChildren = false)
    {
        var res = new List<Nav>();

        foreach (var nav in tree)
        {
            if (!(nav.HasChildren && excludeNavHasChildren))
            {
                res.Add(nav);
            }

            if (nav.HasChildren)
            {
                res.AddRange(FlattenNavs(nav.Children, excludeNavHasChildren));
            }
        }

        return res;
    }

    private async Task CustomErrorHandleAsync(Exception exception)
    {
        GlobalConfig.Loading = false;
        await PopupService.ToastErrorAsync(exception.Message);
    }
}
