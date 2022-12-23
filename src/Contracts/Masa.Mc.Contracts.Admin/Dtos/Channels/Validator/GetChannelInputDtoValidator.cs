// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class GetChannelInputDtoValidator : AbstractValidator<GetChannelInputDto>
{
    public GetChannelInputDtoValidator()
    {
        RuleFor(inputDto => inputDto.Type).IsInEnum().WithMessage("ChannelTypeRequired");
    }
}
