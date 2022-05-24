// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.WebsiteMessages;

public partial class WebsiteMessageManagement : AdminCompontentBase
{
    private bool _detailShow = false;

    private void HandleListItemClick()
    {
        _detailShow = true;
    }

    private void HandleDetailBack()
    {
        _detailShow = false;
    }
}
