﻿namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendMessageTaskInputValidator : AbstractValidator<SendMessageTaskInput>
{
    public SendMessageTaskInputValidator()
    {
        RuleFor(input => input.ReceiverType).IsInEnum();
        RuleFor(input => input.Receivers).Must(x => x.Items.Any()).When(x => x.ReceiverType == ReceiverType.Assign);
    }
}