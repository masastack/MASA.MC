// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class InternalReceiverDto
{
    public Guid SubjectId { get; set; }

    public MessageTaskReceiverTypes Type { get; set; }

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public static implicit operator MessageTaskReceiverDto(InternalReceiverDto dto)
    {
        return new MessageTaskReceiverDto
        {
            SubjectId = dto.SubjectId,
            Type = dto.Type,
            Variables = dto.Variables
        };
    }
}
