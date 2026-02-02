// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics.Validator;

public class ChannelSendStatisticsInputDtoValidator : AbstractValidator<ChannelSendStatisticsInputDto>
{
    public ChannelSendStatisticsInputDtoValidator()
    {
        RuleFor(inputDto => inputDto.StartTime).NotEmpty();
        RuleFor(inputDto => inputDto.EndTime).NotEmpty();
        RuleFor(inputDto => inputDto)
            .Must(inputDto => inputDto.EndTime >= inputDto.StartTime)
            .WithMessage("EndTimeMustBeGreaterThanOrEqualStartTime");
        RuleFor(inputDto => inputDto)
            .Must(inputDto => inputDto.StartTime!.Value.AddYears(1) >= inputDto.EndTime!.Value)
            .WithMessage("TimeRangeMustNotExceedOneYear");
    }
}
