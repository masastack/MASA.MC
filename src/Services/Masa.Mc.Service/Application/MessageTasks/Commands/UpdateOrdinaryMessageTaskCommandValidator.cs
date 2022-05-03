// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class UpdateOrdinaryMessageTaskCommandValidator : AbstractValidator<UpdateOrdinaryMessageTaskCommand>
{
    public UpdateOrdinaryMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}