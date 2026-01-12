// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts.Commands;

public record ReceiveYunMasReceiptCommand(YunMasReceiptInput Input) : Command
{
    public YunMasReceiptResultDto Result { get; set; } = default!;
}
