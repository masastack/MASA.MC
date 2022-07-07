// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class WebsiteMessageReceiverImportDto
{
    [ImporterHeader(Name = "账号", IsAllowRepeat = false)]
    [Required(ErrorMessage = "账号是必填的")]
    public string Account { get; set; } = string.Empty;
}
