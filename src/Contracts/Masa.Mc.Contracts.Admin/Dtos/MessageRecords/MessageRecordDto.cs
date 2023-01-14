// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class MessageRecordDto : AuditEntityDto<Guid, Guid>
{
    public Guid UserId { get;  set; }
    public Guid ChannelId { get;  set; }
    public ChannelDto Channel { get; set; } = new();
    public Guid MessageTaskId { get;  set; }
    public Guid MessageTaskHistoryId { get;  set; }
    public string MessageTaskHistoryNo { get; set; } = string.Empty;
    public bool? Success { get;  set; }
    public DateTimeOffset? SendTime { get;  set; }
    public DateTimeOffset? ExpectSendTime { get; set; }
    public string FailureReason { get;  set; } = string.Empty;
    public bool IsDeleted { get;  set; }
    public ExtraPropertyDictionary ExtraProperties { get;  set; } = new();
    public ExtraPropertyDictionary Variables { get;  set; } = new();
    public string DisplayName { get; set; } = string.Empty;
    public MessageRecordUserDto User { get; set; } = new();
    public string ChannelUserIdentity { get; protected set; } = string.Empty;
}
