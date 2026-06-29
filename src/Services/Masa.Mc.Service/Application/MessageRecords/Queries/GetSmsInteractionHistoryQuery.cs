// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Queries;

public record GetSmsInteractionHistoryQuery(string Mobile, Guid ChannelId) : Query<List<SmsInteractionHistoryDto>>
{
    public override List<SmsInteractionHistoryDto> Result { get; set; } = new();
}
