// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class EmailReceiverImportDto
{
    [ImporterHeader(Name = "邮箱", IsAllowRepeat = false)]
    [Required(ErrorMessage = "邮箱是必填的")]
    public string Email { get; set; } = string.Empty;
}
