﻿namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public record UpdateMessageTemplateCommand(Guid MessageTemplateId, MessageTemplateCreateUpdateDto Template) : Command
{
}
