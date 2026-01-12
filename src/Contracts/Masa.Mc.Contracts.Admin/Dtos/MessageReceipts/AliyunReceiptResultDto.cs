// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class AliyunReceiptResultDto
{
    public int Code { get; set; }
    public string Msg { get; set; }

    public AliyunReceiptResultDto(int code, string msg)
    {
        Code = code;
        Msg = msg;
    }
}
