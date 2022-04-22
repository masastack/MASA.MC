namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskCreateUpdateDtoValidator : AbstractValidator<MessageTaskCreateUpdateDto>
{
    public MessageTaskCreateUpdateDtoValidator()
    {
        RuleFor(input => input.EntityType).IsInEnum();
    }
}
