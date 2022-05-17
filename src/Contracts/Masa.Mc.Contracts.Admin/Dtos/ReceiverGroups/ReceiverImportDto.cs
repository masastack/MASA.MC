// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class ReceiverImportDto
{
    [ImporterHeader(Name = "NickName")]
    public string DisplayName { get; set; } = string.Empty;

    [ImporterHeader(Name = "PhoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [ImporterHeader(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}
