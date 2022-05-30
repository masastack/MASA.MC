// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public class GetMessageTemplateListQueryValidator : AbstractValidator<GetMessageTemplateListQuery>
{
    public GetMessageTemplateListQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTemplateInputDtoValidator());
}