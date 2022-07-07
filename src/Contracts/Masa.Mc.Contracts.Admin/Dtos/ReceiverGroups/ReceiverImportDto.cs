// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class ReceiverImportDto
{
    [ImporterHeader(Name = "手机号")]
    public string PhoneNumber { get; set; } = string.Empty;

    [ImporterHeader(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;

    [ImporterHeader(Name = "账号")]
    public string Account { get; set; } = string.Empty;
}
