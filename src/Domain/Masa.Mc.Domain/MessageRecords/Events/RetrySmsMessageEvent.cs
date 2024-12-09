// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageRecords.Events;

public record RetrySmsMessageEvent : RetryMessageEvent
{
    public RetrySmsMessageEvent(Guid messageRecordId)
        : base(messageRecordId)
    {

    }
}