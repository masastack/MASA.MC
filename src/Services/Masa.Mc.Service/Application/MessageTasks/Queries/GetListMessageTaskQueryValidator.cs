// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public class GetListMessageTaskQueryValidator : AbstractValidator<GetListMessageTaskQuery>
{
    public GetListMessageTaskQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTaskInputValidator());
}