// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Events;

public record SendSimpleSmsMessageEvent: SendSimpleMessageEvent
{
    public ExtraPropertyDictionary Variables { get; set; }

    public string SystemId { get; set; } = string.Empty;

    public SendSimpleSmsMessageEvent(string channelUserIdentity, string channelCode, MessageData messageData)
        : base(channelUserIdentity, channelCode, messageData)
    {

    }
}
