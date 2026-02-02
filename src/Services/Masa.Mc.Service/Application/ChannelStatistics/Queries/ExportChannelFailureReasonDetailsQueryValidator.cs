// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public class ExportChannelFailureReasonDetailsQueryValidator : AbstractValidator<ExportChannelFailureReasonDetailsQuery>
{
    public ExportChannelFailureReasonDetailsQueryValidator() =>
        RuleFor(input => input.Input).SetValidator(new ChannelSendStatisticsInputDtoValidator());
}
