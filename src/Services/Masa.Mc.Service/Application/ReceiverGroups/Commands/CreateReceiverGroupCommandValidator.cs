namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;

public class CreateReceiverGroupCommandValidator : AbstractValidator<CreateReceiverGroupCommand>
{
    public CreateReceiverGroupCommandValidator() => RuleFor(cmd => cmd.ReceiverGroup).SetValidator(new ReceiverGroupCreateUpdateDtoValidator());
}