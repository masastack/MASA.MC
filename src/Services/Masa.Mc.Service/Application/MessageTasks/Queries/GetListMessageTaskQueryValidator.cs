namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public class GetListMessageTaskQueryValidator : AbstractValidator<GetListMessageTaskQuery>
{
    public GetListMessageTaskQueryValidator() => RuleFor(inpu => inpu.Input).SetValidator(new GetMessageTaskInputValidator());
}