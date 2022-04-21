namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateCreateUpdateDtoValidator : AbstractValidator<MessageTemplateCreateUpdateDto>
{
    public MessageTemplateCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).NotEmpty();
        RuleFor(input => input.ChannelId).NotEmpty();
        RuleFor(input => input.Status).IsInEnum();
        RuleFor(input => input.AuditStatus).IsInEnum();
        RuleFor(input => input.Sign).NotEmpty().Length(2, 12).Matches("^[A-z0-9\\u4e00-\\u9fa5]*$").When(x => x.ChannelType == ChannelType.Sms);
        RuleFor(input => input.DayLimit).InclusiveBetween(1, 500);
        RuleFor(input => input.TemplateId).NotEmpty().When(x => x.ChannelType == ChannelType.Sms);
    }
}
