// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Template;

public class WeixinMiniProgramTemplate
{
    public string TemplateId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public int Type { get; set; }

    public List<WeixinMiniProgramTemplateItem> Items { get; set; } = new();
}

public class WeixinMiniProgramTemplateItem
{
    public string Code { get; set; } = string.Empty;
}
