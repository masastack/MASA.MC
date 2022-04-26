namespace Masa.Mc.Service.Admin.Application.MessageInfos.Commands;

public class UpdateMessageInfoCommandValidator : AbstractValidator<UpdateMessageInfoCommand>
{
    public UpdateMessageInfoCommandValidator() => RuleFor(cmd => cmd.MessageInfo).SetValidator(new MessageInfoCreateUpdateDtoValidator());
}