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

    public DateTime SendTime { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }
}
