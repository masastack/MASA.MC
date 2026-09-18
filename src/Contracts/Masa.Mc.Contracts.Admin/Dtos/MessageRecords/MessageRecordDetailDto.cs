// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class MessageRecordDetailDto : MessageRecordDto
{
    public MessageRecordContentDto? MessageContent { get; set; }

    public MessageRecordContentSources ContentSource { get; set; }

    public bool IsContentReliable { get; set; }
}
