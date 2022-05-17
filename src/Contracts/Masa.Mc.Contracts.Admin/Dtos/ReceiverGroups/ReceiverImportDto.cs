// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class ReceiverImportDto
{
    [Required(ErrorMessage = "昵称不能为空")]
    [ImporterHeader(Name = "昵称")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [ImporterHeader(Name = "手机号")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [ImporterHeader(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;
}
