// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages.Validator;

public class GetNoticeListInputDtoValidator : AbstractValidator<GetNoticeListInputDto>
{
    public GetNoticeListInputDtoValidator()
    {
        RuleFor(inputDto => inputDto.PageSize).LessThan(100);
    }
}