// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageTasks;

public class MessageTaskService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageTaskService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-task";
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(GetMessageTaskInputDto inputDto)
    {
        return await GetAsync<GetMessageTaskInputDto, PaginatedListDto<MessageTaskDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<MessageTaskDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageTaskDto>($"{id}");
    }

    public async Task CreateAsync(MessageTaskUpsertDto inputDto)
    {
        await PostAsync(string.Empty, inputDto);
    }

    public async Task UpdateAsync(Guid id, MessageTaskUpsertDto inputDto)
    {
        await PutAsync($"{id}", inputDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task SendAsync(SendMessageTaskInputDto inputDto)
    {
        await PostAsync("Send", inputDto);
    }

    public async Task SendTestAsync(SendTestMessageTaskInputDto inputDto)
    {
        await PostAsync("SendTest", inputDto);
    }

    public async Task EnabledAsync(EnabledMessageTaskInputDto inputDto)
    {
        await PostAsync("Enabled", inputDto);
    }

    public async Task DisableAsync(DisableMessageTaskInputDto inputDto)
    {
        await PostAsync("Disable", inputDto);
    }

    public async Task<ImportResultDto<MessageTaskReceiverDto>> ImportReceiversAsync(ImportReceiversDto dto)
    {
        return await PostAsync<ImportReceiversDto, ImportResultDto<MessageTaskReceiverDto>>("ImportReceivers", dto) ?? new();
    }

    public async Task<byte[]> GenerateReceiverImportTemplateAsync(Guid? messageTemplatesId, ChannelTypes channelType)
    {
        var url = $"{nameof(GenerateReceiverImportTemplateAsync)}?channelType={channelType}";
        if (messageTemplatesId.HasValue)
        {
            url += $"&messageTemplatesId={messageTemplatesId}";
        }
        return await GetAsync<byte[]>(url);
    }

    public async Task<List<MessageTaskReceiverDto>> GetMessageTaskReceiverListAsync(string filter = "")
    {
        return await GetAsync<List<MessageTaskReceiverDto>>($"{nameof(GetMessageTaskReceiverListAsync)}?filter={filter}");
    }

    public async Task<long> ResolveReceiversCountAsync(List<MessageTaskReceiverDto> dto)
    {
        return await PostAsync<List<MessageTaskReceiverDto>, long>(nameof(ResolveReceiversCountAsync), dto);
    }
}
