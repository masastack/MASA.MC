// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts.Commands;

public record ReceiveHuaweiReceiptCommand(HuaweiReceiptInput Input) : Command
{
    public HuaweiReceiptResultDto Result { get; set; } = default!;
}
