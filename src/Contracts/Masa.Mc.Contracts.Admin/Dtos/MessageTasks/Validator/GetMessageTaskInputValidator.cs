namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class GetMessageTaskInputValidator : AbstractValidator<GetMessageTaskInput>
{
    public GetMessageTaskInputValidator()
    {
        RuleFor(input => input.EntityType).IsInEnum();
    }
}
