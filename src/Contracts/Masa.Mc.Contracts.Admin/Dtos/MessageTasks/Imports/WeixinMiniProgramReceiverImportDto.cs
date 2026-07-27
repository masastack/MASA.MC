// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Imports;

public class WeixinMiniProgramReceiverImportDto
{
    [ImporterHeader(Name = "OpenId", IsAllowRepeat = false)]
    [Required(ErrorMessage = "OpenId是必填的")]
    public string OpenId { get; set; } = string.Empty;
}
