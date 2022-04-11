namespace MASA.MC.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateCreateUpdateDtoValidator : AbstractValidator<MessageTemplateCreateUpdateDto>
{
    public MessageTemplateCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).NotEmpty();
    }
}
