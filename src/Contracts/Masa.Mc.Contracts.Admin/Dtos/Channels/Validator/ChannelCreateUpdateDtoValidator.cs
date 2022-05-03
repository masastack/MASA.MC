// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class ChannelCreateUpdateDtoValidator : AbstractValidator<ChannelCreateUpdateDto>
{
    public ChannelCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).Required().Length(2, 50);
        RuleFor(input => input.Code).Required().Length(2, 50);
        RuleFor(input => input.Type).IsInEnum();
        RuleFor(input => input.Description).Length(0, 255);
    }
}
