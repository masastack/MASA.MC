// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class CreateTemplateMessageTaskCommandValidator : AbstractValidator<CreateTemplateMessageTaskCommand>
{
    public CreateTemplateMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskUpsertDtoValidator());
}