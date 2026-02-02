// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public class GetChannelSendTrendQueryValidator : AbstractValidator<GetChannelSendTrendQuery>
{
    public GetChannelSendTrendQueryValidator() =>
        RuleFor(input => input.Input).SetValidator(new ChannelSendStatisticsInputDtoValidator());
}
