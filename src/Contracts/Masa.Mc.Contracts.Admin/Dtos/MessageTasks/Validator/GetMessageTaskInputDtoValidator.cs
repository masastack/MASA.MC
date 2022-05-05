// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class GetMessageTaskInputDtoValidator : AbstractValidator<GetMessageTaskInputDto>
{
    public GetMessageTaskInputDtoValidator()
    {
        RuleFor(inputDto => inputDto.EntityType).IsInEnum();
    }
}
