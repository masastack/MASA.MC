﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.WorkWebhook;

public interface IWeixinWorkWebhookOptions : IOptions
{
    public string Key { get; set; }
}
