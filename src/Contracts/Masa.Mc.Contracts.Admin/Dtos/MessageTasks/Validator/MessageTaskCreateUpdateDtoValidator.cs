namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskCreateUpdateDtoValidator : AbstractValidator<MessageTaskCreateUpdateDto>
{
    public MessageTaskCreateUpdateDtoValidator()
    {
        RuleFor(input => input.EntityId).Required();
        RuleFor(input => input.EntityType).IsInEnum();
        RuleFor(input => input.ReceiverType).IsInEnum();
        RuleFor(input => input.Receivers).Required().When(x=>x.ReceiverType== ReceiverType.Assign);
    }
}
