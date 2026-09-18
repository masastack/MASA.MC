// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Queries;

public record GetMessageRecordQuery(Guid MessageRecordId) : Query<MessageRecordDetailDto>
{
    public override MessageRecordDetailDto Result { get; set; } = new();
}
