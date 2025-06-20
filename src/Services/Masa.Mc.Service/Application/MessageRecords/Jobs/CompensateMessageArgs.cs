// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Jobs;

public class CompensateMessageArgs
{
    public Guid UserId { get; set; }

    public string ChannelCode { get; set; }

    public string TemplateCode { get; set; }

    public ExtraPropertyDictionary Variables { get; set; }
}
