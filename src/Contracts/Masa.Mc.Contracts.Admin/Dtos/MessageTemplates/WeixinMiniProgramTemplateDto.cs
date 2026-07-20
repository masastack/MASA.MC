// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class WeixinMiniProgramTemplateDto
{
    public Guid ChannelId { get; set; }

    public string TemplateId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public int TemplateType { get; set; }

    public List<MessageTemplateItemDto> Items { get; set; } = new();
}
