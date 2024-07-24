// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class WeixinWorkReceiverImportDto
{
    [ImporterHeader(Name = "用户名", IsAllowRepeat = false)]
    [Required(ErrorMessage = "用户名是必填的")]
    public string Account { get; set; }
}
