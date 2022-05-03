// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class GetChannelInputValidator : AbstractValidator<GetChannelInput>
{
    public GetChannelInputValidator()
    {
        RuleFor(input => input.Type).IsInEnum();
    }
}
