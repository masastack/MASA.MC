// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks;

public partial class SendMessage : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnNavigateToList { get; set; }

    private OrdinaryMessageCreateModal _ordinaryCreateModal = default!;
    private TemplateMessageCreateModal _templateCreateModal = default!;

    private async Task NavigateToList()
    {
        if (OnNavigateToList.HasDelegate)
        {
            await OnNavigateToList.InvokeAsync();
        }
        //NavigationManager.NavigateTo("/messageTasks/list");
    }
}
