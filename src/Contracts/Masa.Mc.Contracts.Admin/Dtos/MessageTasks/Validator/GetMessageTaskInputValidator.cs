// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class GetMessageTaskInputValidator : AbstractValidator<GetMessageTaskInput>
{
    public GetMessageTaskInputValidator()
    {
        RuleFor(input => input.EntityType).IsInEnum();
    }
}
