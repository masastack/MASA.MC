// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class VivoReceiptStatusDto
{
    public string Targets { get; set; } = string.Empty; 

    public long AckTime { get; set; }

    public string? Param { get; set; }

    public string? AckType { get; set; }

    public string? SubAckType { get; set; }
}
