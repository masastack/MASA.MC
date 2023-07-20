// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageTasks;

public class MessageTaskHistoryService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageTaskHistoryService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/message-task-history";
    }

    public async Task<PaginatedListDto<MessageTaskHistoryDto>> GetListAsync(GetMessageTaskHistoryInputDto inputDto)
    {
        return await GetAsync<GetMessageTaskHistoryInputDto, PaginatedListDto<MessageTaskHistoryDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<MessageTaskHistoryDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageTaskHistoryDto>($"{id}");
    }

    public async Task<List<MessageTaskReceiverDto>> GetReceiverUsersAsync(Guid id)
    {
        return await GetAsync<List<MessageTaskReceiverDto>>($"{id}/receiverUsers") ?? new();
    }

    public async Task WithdrawnAsync(Guid id)
    {
        await PostAsync($"{id}/Withdrawn", new { });
    }
}
