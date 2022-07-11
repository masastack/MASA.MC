using System.Diagnostics.CodeAnalysis;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components.Routing;

namespace Masa.Mc.Web.Admin.Layouts;

public partial class McLayout
{
    [Inject]
    [NotNull]
    public IPopupService PopupService { get; set; }

    [Inject]
    private I18n I18n { get; set; } = null!;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter, EditorRequired]
    public string? Logo { get; set; }

    [Parameter, EditorRequired]
    public string? MiniLogo { get; set; }

    [Parameter, EditorRequired]
    public string AppId { get; set; } = string.Empty;

    [Parameter]
    public string UserCenterRoute { get; set; } = "/user-center";

    [Parameter]
    public EventCallback OnSignOut { get; set; }

    [Parameter]
    public Func<Exception, Task>? OnErrorAsync { get; set; }

    [Parameter]
    public RenderFragment<Exception>? ErrorContent { get; set; }

    List<Nav> NavItems = new();

    List<Nav> FlattenedNavs { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var menus = await AuthClient.PermissionService.GetMenusAsync(AppId);
            NavItems = menus.Adapt<List<Nav>>();
            if (!NavItems.Any())
            {
                NavItems = new List<Nav>()
            {
                new Nav("channelManagement", "Permission.ChannelManagement", "mdi-email-outline", "channels/channelManagement", 1),
                new Nav("messageManagement", "Permission.MessageManagement", "fas fa-tasks", 1, new List<Nav>
                {
                    new Nav("sendMessage", "Permission.SendMessage", "messageTasks/sendMessage", 2, "messageManagement"),
                    new Nav("messageRecord", "Permission.MessageRecord", "messageRecords/messageRecordManagement", 2, "messageManagement"),
                }),
                new Nav("messageTemplateManagement", "Permission.MessageTemplateManagement", "mdi-collage", 1, new List<Nav>
                {
                    new Nav("sms", "Sms", "messageTemplates/smsTemplateManagement", 2, "messageTemplateManagement"),
                    new Nav("email", "Email", "messageTemplates/emailTemplateManagement", 2, "messageTemplateManagement"),
                    new Nav("websiteMessage", "WebsiteMessage", "messageTemplates/websiteMessageTemplateManagement", 2, "messageTemplateManagement"),
                }),
                new Nav("receiverGroupManagement", "Permission.ReceiverGroupManagement", "fas fa-object-ungroup", "receiverGroups/receiverGroupManagement", 1),
            };
            }

            FlattenedNavs = FlattenNavs(NavItems, true);
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

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

    protected override void OnInitialized()
    {
        OnErrorAsync ??= async exception =>
        {
            await PopupService.ToastErrorAsync(exception.Message);
        };

        PopupService.ConfigToast(config =>
        {
            config.Position = ToastPosition.TopLeft;
        });

        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        AuthClient.UserService.VisitedAsync(e.Location);
    }

    private async Task AddFavoriteMenu(string code)
    {
        await AuthClient.PermissionService.AddFavoriteMenuAsync(Guid.Parse(code));
    }

    private async Task RemoveFavoriteMenu(string code)
    {
        await AuthClient.PermissionService.RemoveFavoriteMenuAsync(Guid.Parse(code));
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}
