// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Work;

public class WeixinWorkOptions : IWeixinWorkOptions
{
    public string CorpId { get; set; } = string.Empty;

    public string CorpSecret { get; set; } = string.Empty;

    public string AgentId { get; set; } = string.Empty;
}
