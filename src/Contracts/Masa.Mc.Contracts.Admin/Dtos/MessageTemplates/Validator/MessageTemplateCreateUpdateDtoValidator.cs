namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateCreateUpdateDtoValidator : AbstractValidator<MessageTemplateCreateUpdateDto>
{
    public MessageTemplateCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).NotEmpty();
        RuleFor(input => input.ChannelType).IsInEnum();
        RuleFor(input => input.ChannelId).NotEmpty();
        RuleFor(input => input.Status).IsInEnum();
        RuleFor(input => input.AuditStatus).IsInEnum();
    }
}
