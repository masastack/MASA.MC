﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public class GetListMessageTemplateQueryValidator : AbstractValidator<GetListMessageTemplateQuery>
{
    public GetListMessageTemplateQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTemplateInputDtoValidator());
}