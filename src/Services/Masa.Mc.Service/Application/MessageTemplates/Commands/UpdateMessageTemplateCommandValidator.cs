namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public class UpdateMessageTemplateCommandValidator : AbstractValidator<UpdateMessageTemplateCommand>
{
    public UpdateMessageTemplateCommandValidator() => RuleFor(cmd => cmd.MessageTemplate).SetValidator(new MessageTemplateCreateUpdateDtoValidator());
}