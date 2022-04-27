namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public class CreateOrdinaryMessageTaskCommandValidator : AbstractValidator<CreateOrdinaryMessageTaskCommand>
{
    public CreateOrdinaryMessageTaskCommandValidator() => RuleFor(cmd => cmd.MessageTask).SetValidator(new MessageTaskCreateUpdateDtoValidator());
}