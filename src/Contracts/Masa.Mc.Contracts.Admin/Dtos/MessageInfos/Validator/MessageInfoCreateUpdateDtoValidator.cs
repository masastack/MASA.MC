// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;

public class MessageInfoCreateUpdateDtoValidator : AbstractValidator<MessageInfoCreateUpdateDto>
{
    public MessageInfoCreateUpdateDtoValidator()
    {
        RuleFor(inputDto => inputDto.Title).Required();
        RuleFor(inputDto => inputDto.Content).Required();
    }
}
