// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageInfos;

public class MessageInfoQueryHandler
{
    private readonly IMcQueryContext _context;

    public MessageInfoQueryHandler(IMcQueryContext context)
    {
        _context = context;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageInfoQuery query)
    {
        var entity = await _context.MessageInfoQueries.FirstOrDefaultAsync(x => x.Id == query.MessageInfoId);
        MasaArgumentException.ThrowIfNull(entity, "MessageInfo");

        query.Result = entity.Adapt<MessageInfoDto>();
    }
}
