// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Caller.Services.ReceiverGroups;

public class ReceiverGroupService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public ReceiverGroupService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/receiver-group";
    }

    public async Task<PaginatedListDto<ReceiverGroupDto>> GetListAsync(GetReceiverGroupInputDto inputDto)
    {
        return await GetAsync<GetReceiverGroupInputDto, PaginatedListDto<ReceiverGroupDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<ReceiverGroupDto?> GetAsync(Guid id)
    {
        return await GetAsync<ReceiverGroupDto>($"{id}");
    }

    public async Task CreateAsync(ReceiverGroupCreateUpdateDto inputDto)
    {
        await PostAsync(string.Empty, inputDto);
    }

    public async Task UpdateAsync(Guid id, ReceiverGroupCreateUpdateDto inputDto)
    {
        await PutAsync($"{id}", inputDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }
}
