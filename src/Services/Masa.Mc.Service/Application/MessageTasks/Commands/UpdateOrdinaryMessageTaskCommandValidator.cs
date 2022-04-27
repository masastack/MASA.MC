namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class UpdateOrdinaryMessageTaskCommandValidator : AbstractValidator<UpdateOrdinaryMessageTaskCommand>
{
    public UpdateOrdinaryMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}