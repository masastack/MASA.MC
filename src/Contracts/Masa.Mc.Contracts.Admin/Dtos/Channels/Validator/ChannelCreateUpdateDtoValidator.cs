// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class ChannelCreateUpdateDtoValidator : AbstractValidator<ChannelCreateUpdateDto>
{
    public ChannelCreateUpdateDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required().Length(2, 50);
        RuleFor(inputDto => inputDto.Code).Required().Length(2, 50);
        RuleFor(inputDto => inputDto.Type).IsInEnum();
        RuleFor(inputDto => inputDto.Description).Length(0, 255);
    }
}
