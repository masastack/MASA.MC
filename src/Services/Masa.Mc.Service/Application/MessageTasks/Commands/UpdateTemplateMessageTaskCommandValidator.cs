namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class UpdateTemplateMessageTaskCommandValidator : AbstractValidator<UpdateTemplateMessageTaskCommand>
{
    public UpdateTemplateMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}