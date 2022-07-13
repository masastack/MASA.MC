// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageSendingRules : AdminCompontentBase
{
    [Parameter]
    public SendRuleDto Value { get; set; } = new();

    [Parameter]
    public EventCallback<SendRuleDto?> ValueChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }
}
