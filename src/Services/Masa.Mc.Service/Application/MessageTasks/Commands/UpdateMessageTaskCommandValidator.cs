namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class UpdateMessageTaskCommandValidator : AbstractValidator<UpdateMessageTaskCommand>
{
    public UpdateMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}