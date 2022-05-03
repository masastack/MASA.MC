﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageInfoService : ServiceBase
{
    public MessageInfoService(IServiceCollection services) : base(services, "api/message-info")
    {
        MapGet(GetAsync, "{id}");
    }

    public async Task<MessageInfoDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageInfoQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
