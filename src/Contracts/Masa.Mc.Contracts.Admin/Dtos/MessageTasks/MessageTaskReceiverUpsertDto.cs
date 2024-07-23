// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskReceiverUpsertDto
{
    public Guid SubjectId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public MessageTaskReceiverTypes Type { get; set; }

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
