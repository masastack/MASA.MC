namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;

public class UpdateReceiverGroupCommandValidator : AbstractValidator<UpdateReceiverGroupCommand>
{
    public UpdateReceiverGroupCommandValidator() => RuleFor(cmd => cmd.ReceiverGroup).SetValidator(new ReceiverGroupCreateUpdateDtoValidator());
}