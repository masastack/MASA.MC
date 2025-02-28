﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class SendSimpleMessageArgs
{
    public string ChannelCode { get; set; }

    public ChannelTypes ChannelType { get; set; }

    public string TemplateCode { get; set; }

    public string ChannelUserIdentity { get; set; }

    public ExtraPropertyDictionary Variables { get; set; }

    public string SystemId { get; set; } = string.Empty;
}
