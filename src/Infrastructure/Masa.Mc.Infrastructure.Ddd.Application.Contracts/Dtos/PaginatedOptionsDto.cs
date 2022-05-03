// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;

public class PaginatedOptionsDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string Sorting { get; set; }

    public PaginatedOptionsDto(string sorting = "", int page = 1, int pageSize = 20)
    {
        Sorting = sorting;
        Page = page;
        PageSize = pageSize;
    }
}
