// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.Unsubscriptions;

public class UnsubscriptionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UnsubscriptionService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/unsubscription";
    }

    public async Task<PaginatedListDto<UnsubscriptionDto>> GetListAsync(GetUnsubscriptionInputDto inputDto)
    {
        return await GetAsync<GetUnsubscriptionInputDto, PaginatedListDto<UnsubscriptionDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<PaginatedListDto<UnsubscriptionHistoryDto>> GetHistoryListAsync(GetUnsubscriptionHistoryInputDto inputDto)
    {
        return await GetAsync<GetUnsubscriptionHistoryInputDto, PaginatedListDto<UnsubscriptionHistoryDto>>("history", inputDto) ?? new();
    }

    public async Task<UnsubscriptionDetailDto?> GetAsync(Guid id)
    {
        return await GetAsync<UnsubscriptionDetailDto>($"{id}");
    }
}
