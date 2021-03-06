// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Pagination;

public class PaginationEventArgs
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public PaginationEventArgs(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public void Deconstruct(out int page, out int pageSize)
    {
        page = Page;
        pageSize = PageSize;
    }
}
