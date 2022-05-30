// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;

public class WebsiteMessageDto : AuditEntityDto<Guid, Guid>
{
    public Guid ChannelId { get; set; }

    public ChannelDto Channel { get; set; } = default!;

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadTime { get; set; }

    public string Abstract { get; set; } = string.Empty;

    public Guid PrevId { get; set; }

    public Guid NextId { get; set; }
}
