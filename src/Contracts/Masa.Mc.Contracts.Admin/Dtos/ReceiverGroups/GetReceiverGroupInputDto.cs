// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class GetReceiverGroupInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;

    public GetReceiverGroupInputDto()
    {

    }

    public GetReceiverGroupInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetReceiverGroupInputDto(string filter, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
    }
}
