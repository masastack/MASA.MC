namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;

public class MessageInfoCreateUpdateDtoValidator : AbstractValidator<MessageInfoCreateUpdateDto>
{
    public MessageInfoCreateUpdateDtoValidator()
    {
        RuleFor(input => input.Title).Required();
        RuleFor(input => input.Content).Required();
    }
}
