// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class GetSmsInteractionHistoryInputDto
{
    public string Mobile { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }
}
