// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class SmsReceiverImportDto
{
    [ImporterHeader(Name = "手机号", IsAllowRepeat = false)]
    [Required(ErrorMessage = "手机号是必填的")]
    public string PhoneNumber { get; set; } = string.Empty;
}
