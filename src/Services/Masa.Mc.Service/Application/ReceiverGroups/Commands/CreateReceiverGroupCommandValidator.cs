// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;

public class CreateReceiverGroupCommandValidator : AbstractValidator<CreateReceiverGroupCommand>
{
    public CreateReceiverGroupCommandValidator() => RuleFor(cmd => cmd.ReceiverGroup).SetValidator(new ReceiverGroupCreateUpdateDtoValidator());
}