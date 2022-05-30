// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public class GetMessageTaskListQueryValidator : AbstractValidator<GetMessageTaskListQuery>
{
    public GetMessageTaskListQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTaskInputDtoValidator());
}