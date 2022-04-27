namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class CreateTemplateMessageTaskCommandValidator : AbstractValidator<CreateTemplateMessageTaskCommand>
{
    public CreateTemplateMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}