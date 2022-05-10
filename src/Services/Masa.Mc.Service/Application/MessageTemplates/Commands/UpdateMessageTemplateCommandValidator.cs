// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public class UpdateMessageTemplateCommandValidator : AbstractValidator<UpdateMessageTemplateCommand>
{
    public UpdateMessageTemplateCommandValidator() => RuleFor(cmd => cmd.MessageTemplate).SetValidator(new MessageTemplateUpsertDtoValidator());
}