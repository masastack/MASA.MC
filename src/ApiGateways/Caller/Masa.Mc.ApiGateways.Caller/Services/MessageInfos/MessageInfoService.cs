// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageInfos;

public class MessageInfoService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageInfoService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/message-info";
    }

    public async Task<MessageInfoDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageInfoDto>($"{id}");
    }
}
