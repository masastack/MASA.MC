namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class CreateMessageTaskCommandValidator : AbstractValidator<CreateMessageTaskCommand>
{
    public CreateMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}