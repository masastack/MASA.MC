﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class AppReceiverImportDto
{
    [ImporterHeader(Name = "客户端注册Id", IsAllowRepeat = false)]
    [Required(ErrorMessage = "客户端注册Id是必填的")]
    public string ClientId { get; set; }
}
