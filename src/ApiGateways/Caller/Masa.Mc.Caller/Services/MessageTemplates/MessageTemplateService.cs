﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Caller.Services.MessageTemplates;

public class MessageTemplateService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageTemplateService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-template";
    }

    public async Task<PaginatedListDto<MessageTemplateDto>> GetListAsync(GetMessageTemplateInputDto inputDto)
    {
        return await GetAsync<GetMessageTemplateInputDto,PaginatedListDto<MessageTemplateDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<MessageTemplateDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageTemplateDto>($"{id}");
    }

    public async Task CreateAsync(MessageTemplateCreateUpdateDto inputDto)
    {
        await PostAsync(string.Empty, inputDto);
    }

    public async Task UpdateAsync(Guid id, MessageTemplateCreateUpdateDto inputDto)
    {
        await PutAsync($"{id}", inputDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task<MessageTemplateDto?> FindByCodeAsync(string code)
    {
        return await GetAsync<MessageTemplateDto>($"FindByCode?code={code}");
    }
}
