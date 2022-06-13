// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks;

public partial class SendMessage : AdminCompontentBase
{
    private OrdinaryMessageCreateModal _ordinaryCreateModal = default!;
    private TemplateMessageCreateModal _templateCreateModal = default!;
    private MessageTaskListModal _listModal = default!;

    private void NavigateToList()
    {
        NavigationManager.NavigateTo("/messageTasks/list");
    }
}
