// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public class GetChannelFailureReasonOverviewQueryValidator : AbstractValidator<GetChannelFailureReasonOverviewQuery>
{
    public GetChannelFailureReasonOverviewQueryValidator() =>
        RuleFor(input => input.Input).SetValidator(new ChannelSendStatisticsInputDtoValidator());
}
