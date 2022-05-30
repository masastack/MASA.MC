// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public class GetChannelListQueryValidator : AbstractValidator<GetChannelListQuery>
{
    public GetChannelListQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetChannelInputDtoValidator());
}