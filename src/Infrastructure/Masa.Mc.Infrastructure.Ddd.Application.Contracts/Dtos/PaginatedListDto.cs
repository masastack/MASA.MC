// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;

public class PaginatedListDto<T>
{
    public long Total { get; set; }

    public int TotalPages { get; set; }

    public List<T> Result { get; set; } = default!;

    public PaginatedListDto()
    {
        Result = new List<T>();
    }

    public PaginatedListDto(long total, int totalPages, List<T> result)
    {
        Total = total;
        TotalPages = totalPages;
        Result = result;
    }
}
