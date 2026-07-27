// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class WeixinMiniProgramReceiptInput
{
    public string OpenId { get; set; } = string.Empty;

    public string TemplateId { get; set; } = string.Empty;

    public string MsgId { get; set; } = string.Empty;

    public string ErrorCode { get; set; } = string.Empty;

    public string ErrorStatus { get; set; } = string.Empty;

    public DateTimeOffset ReceiveTime { get; set; } = DateTimeOffset.UtcNow;
}
