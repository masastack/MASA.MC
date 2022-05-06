// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public record CreateMessageTemplateCommand(MessageTemplateUpsertDto MessageTemplate) : Command
{
}
