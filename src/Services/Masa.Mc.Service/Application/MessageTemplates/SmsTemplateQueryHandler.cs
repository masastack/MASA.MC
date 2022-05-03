// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class SmsTemplateQueryHandler
{
    private readonly ISmsTemplateRepository _repository;

    public SmsTemplateQueryHandler(ISmsTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetListAsync(GetSmsTemplateListByChannelIdQuery query)
    {
        var resultList = await _repository.GetListAsync(x => x.ChannelId == query.ChannelId);
        var dtos = resultList.Adapt<List<SmsTemplateDto>>();
        query.Result = dtos;
    }
}
