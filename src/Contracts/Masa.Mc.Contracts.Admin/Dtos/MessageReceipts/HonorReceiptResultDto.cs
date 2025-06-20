// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class HonorReceiptResultDto
{
    public string Code { get; set; }
    public string Message { get; set; }

    public HonorReceiptResultDto(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
