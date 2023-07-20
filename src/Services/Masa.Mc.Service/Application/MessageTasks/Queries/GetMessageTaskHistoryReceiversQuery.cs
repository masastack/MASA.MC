// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetMessageTaskHistoryReceiversQuery(Guid MessageTaskHistoryId) : Query<List<MessageTaskReceiverDto>>
{
    public override List<MessageTaskReceiverDto> Result { get; set; } = new();
}