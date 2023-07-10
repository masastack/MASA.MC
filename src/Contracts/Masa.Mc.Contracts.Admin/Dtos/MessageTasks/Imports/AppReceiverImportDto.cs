// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class AppReceiverImportDto
{
    [ImporterHeader(Name = "用户Id", IsAllowRepeat = false)]
    [Required(ErrorMessage = "用户Id是必填的")]
    public Guid UserId { get; set; }
}
