// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class CompensateMessageEventHandler
{
    public CompensateMessageEventHandler()
    {
    }

    [EventHandler]
    public async Task HandleEventAsync(CompensateMessageEvent eto)
    {
        var args = new CompensateMessageArgs()
        {
            UserId = eto.UserId,
            ChannelCode = eto.ChannelCode,
            TemplateCode = eto.TemplateCode,
            Variables = eto.Variables
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
