// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageInfos;

public class MessageInfoQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly II18n<DefaultResource> _i18n;

    public MessageInfoQueryHandler(IMcQueryContext context, II18n<DefaultResource> i18n)
    {
        _context = context;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageInfoQuery query)
    {
        var entity = await _context.MessageInfoQueries.FirstOrDefaultAsync(x => x.Id == query.MessageInfoId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageInfo"));

        query.Result = entity.Adapt<MessageInfoDto>();
    }
}
