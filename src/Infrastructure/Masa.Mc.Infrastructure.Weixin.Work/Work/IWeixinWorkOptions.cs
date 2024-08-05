// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Work;

public interface IWeixinWorkOptions
{
    public string CorpId { get; set; }

    public string CorpSecret { get; set; }

    public string AgentId { get; set; }
}