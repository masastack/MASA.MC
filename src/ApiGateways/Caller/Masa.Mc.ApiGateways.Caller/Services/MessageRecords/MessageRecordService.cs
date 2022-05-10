// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageRecords;

public class MessageRecordService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageRecordService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-record";
    }

    public async Task<PaginatedListDto<MessageRecordDto>> GetListAsync(GetMessageRecordInputDto inputDto)
    {
        return await GetAsync<GetMessageRecordInputDto, PaginatedListDto<MessageRecordDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<MessageRecordDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageRecordDto>($"{id}");
    }
}
