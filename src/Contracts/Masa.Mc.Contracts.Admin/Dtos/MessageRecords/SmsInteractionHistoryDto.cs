// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class SmsInteractionHistoryDto
{
    public DateTimeOffset SendTime { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool IsInbound { get; set; }
}
