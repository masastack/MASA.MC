namespace MASA.MC.Service.Admin.Application.MessageTemplates.Commands;

public class CreateMessageTemplateCommandValidator : AbstractValidator<CreateMessageTemplateCommand>
{
    public CreateMessageTemplateCommandValidator() => RuleFor(cmd => cmd.MessageTemplate).SetValidator(new MessageTemplateCreateUpdateDtoValidator());
}