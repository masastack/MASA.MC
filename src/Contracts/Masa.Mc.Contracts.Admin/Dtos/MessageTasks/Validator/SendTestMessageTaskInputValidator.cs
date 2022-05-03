// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendTestMessageTaskInputValidator : AbstractValidator<SendTestMessageTaskInput>
{
    public SendTestMessageTaskInputValidator()
    {
        RuleFor(input => input.Receivers).Must(x => x.Items.Any());
    }
}