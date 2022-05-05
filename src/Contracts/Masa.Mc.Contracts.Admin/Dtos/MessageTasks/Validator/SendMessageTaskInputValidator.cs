// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendMessageTaskInputValidator : AbstractValidator<SendMessageTaskInput>
{
    public SendMessageTaskInputValidator()
    {
        RuleFor(input => input.ReceiverType).IsInEnum();
        RuleFor(input => input.Receivers).Required().When(x => x.ReceiverType == ReceiverType.Assign);
    }
}